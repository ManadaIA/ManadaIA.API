# 🐄 ManadaIA API

API REST para gestão genética e reprodutiva de rebanhos (bovinos, ovinos e caprinos) com análise de Inteligência Artificial para predição de prenhez.

---

## 🎯 Sobre o Projeto

Sistema desenvolvido para criadores do sertão cearense que permite:

- 📋 Cadastro e gestão de animais (bovinos, ovinos, caprinos)
- 🔄 Registro de ciclos reprodutivos (inseminação, parto, diagnóstico)
- 🤖 Predições de prenhez usando IA (Gemini API)
- 📊 Relatórios de desempenho reprodutivo
- 🔐 Autenticação segura via Supabase Auth

---

## 🏗️ Arquitetura

### Clean Architecture com Service + Repository Pattern

```
┌─────────────────────────────────────────┐
│     API (Controllers + Middleware)      │
├─────────────────────────────────────────┤
│  Application (Services + DTOs)          │
├─────────────────────────────────────────┤
│  Domain (Entities + Interfaces)         │
├─────────────────────────────────────────┤
│  Infrastructure (Repositories + DB)     │
└─────────────────────────────────────────┘
```

---

## 🚀 Stack Tecnológico

- **.NET 8** - Framework principal
- **Supabase** - Autenticação JWT + PostgreSQL
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API

---

## 📦 Estrutura do Projeto

```
ManadaIA.API/
├── src/
│   ├── ManadaIA.API/              # Camada de apresentação
│   │   ├── Controllers/           # Endpoints REST
│   │   ├── Middleware/            # Exception handling
│   │   └── Extensions/            # Configurações
│   │
│   ├── ManadaIA.Application/      # Lógica de negócio
│   │   ├── Services/              # Services principais
│   │   └── DTOs/                  # Data Transfer Objects
│   │
│   ├── ManadaIA.Domain/           # Domínio central
│   │   ├── Entities/              # Entidades de negócio
│   │   ├── Interfaces/            # Contratos dos repositórios
│   │   └── Exceptions/            # Exceções customizadas
│   │
│   └── ManadaIA.Infrastructure/   # Infraestrutura
│       ├── Repositories/          # Implementações Supabase
│       ├── Models/                # Models de persistência
│       └── Supabase/              # Configuração Supabase
│
├── database/
│   └── schema.sql                 # Script de criação do banco
│
├── api-examples.http              # Exemplos de requisições
```

---

## 🗄️ Modelo de Dados

### Principais Entidades

#### 1. **Animal**
- Informações básicas: código, nome, espécie, sexo, raça
- Linhagem, data de nascimento, peso
- Suporta: bovinos, ovinos, caprinos

#### 2. **ReproductiveCycle**
- Histórico reprodutivo completo
- Eventos: inseminação, parto, diagnóstico
- Técnicas: IATF, IA convencional, monta natural
- Resultados: prenha, vazia, aguardando

#### 3. **AIPrediction**
- Predições de prenhez geradas por IA
- Taxa de sucesso (0-100%)
- Nível de confiança (alta, média, baixa)
- Fatores de risco e recomendações

---

## 🔌 Endpoints da API

### 🐄 Animals
```http
GET    /api/v1/animals                  # Lista todos os animais
GET    /api/v1/animals/{id}             # Detalhe do animal
GET    /api/v1/animals/species/{sp}     # Filtra por espécie
POST   /api/v1/animals                  # Cadastra animal
PUT    /api/v1/animals/{id}             # Atualiza animal
DELETE /api/v1/animals/{id}             # Remove animal
```

### 🔄 Reproductive Cycles
```http
GET    /api/v1/cycles/animals/{id}      # Histórico do animal
POST   /api/v1/cycles                   # Registra evento
PUT    /api/v1/cycles/{id}              # Atualiza resultado
```

### 🤖 AI Predictions
```http
POST   /api/v1/ai/predict               # Gera predição
GET    /api/v1/ai/predictions/{id}      # Detalhe da predição
GET    /api/v1/ai/predictions/animal/{id} # Histórico
```

### 📊 Reports
```http
GET    /api/v1/reports/summary          # Resumo geral
GET    /api/v1/reports/pregnancy-rate   # Taxa de prenhez
```

---

## ⚙️ Configuração

### 1. Pré-requisitos

- .NET 8 SDK
- Conta no Supabase (gratuita)
- Visual Studio 2022 ou VS Code

### 2. Configurar Supabase

1. Crie um projeto em [supabase.com](https://supabase.com)
2. Execute o script `database/schema.sql` no SQL Editor
3. Copie as credenciais do projeto

### 3. Configurar appsettings.json

```json
{
  "Supabase": {
    "Url": "https://seu-projeto.supabase.co",
    "Key": "sua-anon-key",
    "Secret": "sua-service-role-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### 4. Executar o Projeto

```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Executar
cd src/ManadaIA.API
dotnet run
```

### 5. Acessar a API

- **Swagger UI**: https://localhost:5001/swagger
- **Health Check**: https://localhost:5001/health

---

## 🧪 Testando a API

Use o arquivo `api-examples.http` com a extensão REST Client do VS Code:

```http
### Criar um animal
POST https://localhost:5001/api/v1/animals
Authorization: Bearer SEU_TOKEN
Content-Type: application/json

{
  "code": "BOV-001",
  "name": "Mimosa",
  "species": "bovino",
  "sex": "femea",
  "breed": "Nelore",
  "birthDate": "2022-03-15",
  "weightKg": 450.5
}
```

---

## 🔐 Autenticação

Todas as rotas requerem autenticação JWT via Supabase:

```http
Authorization: Bearer <seu_token_jwt>
```

O token é obtido através do login no Supabase Auth.

---

## 📝 Licença

**GNU GPL v3.0** - Código-fonte aberto conforme edital do projeto.

---

## 👥 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 📚 Documentação Adicional

- [api-examples.http](api-examples.http) - Exemplos completos de requisições
- [database/schema.sql](database/schema.sql) - Schema completo do banco

---

## 🐛 Reportar Bugs

Abra uma issue no GitHub com:
- Descrição do problema
- Passos para reproduzir
- Comportamento esperado vs. atual
- Screenshots (se aplicável)

---

## 📧 Contato

Para dúvidas ou sugestões sobre o projeto, abra uma issue ou discussion no GitHub.

---

**Desenvolvido para produtores rurais do sertão cearense 🌵**
