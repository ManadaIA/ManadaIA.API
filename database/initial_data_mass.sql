
-- ═══════════════════════════════════════════════════════════════
-- ManadaIA - Schema do Banco de Dados Supabase
-- Sistema de Gestão de Rebanho (Bovino, Ovino, Caprino)
-- ═══════════════════════════════════════════════════════════════

-- Execute este script no SQL Editor do Supabase Dashboard após executar o schema.sql

-- ───────────────────────────────────────────────────────────────
-- TABELA: notification_parameters (Configuração Global)
-- ───────────────────────────────────────────────────────────────

INSERT INTO notification_parameters (key, group_type, name, description, is_default_enabled) VALUES
('ALERT_INSEMINATION_SUCCESS', 'WHATSAPP', 'Receber Notificação Após Realizar Inseminação', 'Envia uma confirmação no WhatsApp assim que uma nova inseminação for registrada.', true),
('ALERT_AI_PREDICTION_GENERATED', 'WHATSAPP', 'Receber Notificação de Análises de IA', 'Avisa no WhatsApp quando o modelo de IA gerar um novo relatório de probabilidade de prenhez.', true);

