-- ═══════════════════════════════════════════════════════════════
-- ManadaIA - Schema do Banco de Dados Supabase
-- Sistema de Gestão de Rebanho (Bovino, Ovino, Caprino)
-- ═══════════════════════════════════════════════════════════════

-- Execute este script no SQL Editor do Supabase Dashboard

-- ───────────────────────────────────────────────────────────────
-- 1. TABELA: animals
-- ───────────────────────────────────────────────────────────────
CREATE TABLE animals (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     UUID NOT NULL REFERENCES auth.users(id),
    code        VARCHAR(50) NOT NULL,
    name        VARCHAR(100),
    species     VARCHAR(20) NOT NULL CHECK (species IN ('BOVINO','OVINO','CAPRINO')),
    sex         VARCHAR(10) NOT NULL CHECK (sex IN ('FEMEA','MACHO')),
    breed       VARCHAR(100),
    lineage     TEXT,
    birth_date  DATE,
    weight_kg   NUMERIC(6,2),
    notes       TEXT,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ
);

CREATE INDEX idx_animals_user ON animals(user_id);
CREATE UNIQUE INDEX idx_animals_code ON animals(user_id, code);
CREATE INDEX idx_animals_species ON animals(user_id, species);


-- ───────────────────────────────────────────────────────────────
-- 2. TABELA: reproductive_cycles
-- ───────────────────────────────────────────────────────────────
CREATE TABLE reproductive_cycles (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    animal_id       UUID NOT NULL REFERENCES animals(id) ON DELETE CASCADE,
    event_date      DATE NOT NULL,
    event_type      VARCHAR(30) NOT NULL CHECK (event_type IN ('INSEMINACAO','PARTO','DIAGNOSTICO','RETORNO')),
    sire_name       VARCHAR(100),
    semen_batch     VARCHAR(50),
    technique       VARCHAR(30) CHECK (technique IN ('IATF','IA','MONTA')),
    technician      VARCHAR(100),
    result          VARCHAR(20) CHECK (result IN ('PRENHA','VAZIA','AGUARDANDO','PERDIDA')),
    notes           TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_cycles_animal ON reproductive_cycles(animal_id);
CREATE INDEX idx_cycles_event_date ON reproductive_cycles(event_date DESC);
CREATE INDEX idx_cycles_event_type ON reproductive_cycles(event_type);

-- ───────────────────────────────────────────────────────────────
-- 3. TABELA: ai_predictions
-- ───────────────────────────────────────────────────────────────
CREATE TABLE ai_predictions (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    animal_id           UUID NOT NULL REFERENCES animals(id) ON DELETE CASCADE,
    cycle_id            UUID REFERENCES reproductive_cycles(id) ON DELETE SET NULL,
    prediction_date     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    pregnancy_rate      NUMERIC(5,2) CHECK (pregnancy_rate >= 0 AND pregnancy_rate <= 100),
    confidence_level    VARCHAR(10) CHECK (confidence_level IN ('ALTA','MEDIA','BAIXA')),
    explanation         TEXT,
    risk_factors        TEXT,  -- JSON serializado
    recommendations     TEXT,  -- JSON serializado
    ai_model_used       VARCHAR(50),
    raw_prompt          TEXT,
    raw_response        TEXT
);

CREATE INDEX idx_predictions_animal ON ai_predictions(animal_id);
CREATE INDEX idx_predictions_cycle ON ai_predictions(cycle_id);
CREATE INDEX idx_predictions_date ON ai_predictions(prediction_date DESC);

-- ═══════════════════════════════════════════════════════════════
-- ROW LEVEL SECURITY (RLS) - LGPD
-- ═══════════════════════════════════════════════════════════════

-- Ativar RLS em todas as tabelas
ALTER TABLE animals ENABLE ROW LEVEL SECURITY;
ALTER TABLE reproductive_cycles ENABLE ROW LEVEL SECURITY;
ALTER TABLE ai_predictions ENABLE ROW LEVEL SECURITY;

-- Política: usuário vê apenas seus próprios animais
CREATE POLICY "users_own_animals" ON animals
    FOR ALL USING (auth.uid() = user_id);

-- Política: usuário vê apenas ciclos de seus animais
CREATE POLICY "users_own_cycles" ON reproductive_cycles
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM animals 
            WHERE animals.id = reproductive_cycles.animal_id 
            AND animals.user_id = auth.uid()
        )
    );

-- Política: usuário vê apenas predições de seus animais
CREATE POLICY "users_own_predictions" ON ai_predictions
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM animals 
            WHERE animals.id = ai_predictions.animal_id 
            AND animals.user_id = auth.uid()
        )
    );
