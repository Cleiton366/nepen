\# 🚀 Desafio NEPEN - API



\## 📋 Índice

\- \[Visão Geral](#-visão-geral)

\- \[Tecnologias Utilizadas](#-tecnologias-utilizadas)

\- \[Arquitetura e Decisões Técnicas](#-arquitetura-e-decisões-técnicas)

\- \[Como Executar](#-como-executar)

\- \[Scripts de Banco de Dados](#-scripts-de-banco-de-dados)

\- \[Testes de Performance](#-testes-de-performance)

\- \[Monitoramento e Logs](#-monitoramento-e-logs)



\## 🎯 Visão Geral



API desenvolvida para gerenciamento de leituras de medidores com foco em alta performance, resiliência e escalabilidade. A solução implementa padrões modernos de desenvolvimento e arquitetura limpa.



\## 🛠 Tecnologias Utilizadas



\- \*\*.NET 8\*\* - Framework principal

\- \*\*Entity Framework Core\*\* - ORM para acesso a dados

\- \*\*PostgreSQL\*\* - Banco de dados relacional

\- \*\*Redis\*\* - Cache em memória distribuída

\- \*\*Docker\*\* - Containerização e orquestração

\- \*\*Serilog\*\* - Logging estruturado

\- \*\*FluentValidation\*\* - Validações robustas

\- \*\*Artillery\*\* - Testes de performance



\## 🏗 Arquitetura e Decisões Técnicas



\### Clean Architecture

A aplicação segue os princípios da Clean Architecture, organizando o código em camadas bem definidas:



\- \*\*Core\*\* - Entidades e regras de negócio

\- \*\*Infra\*\* - Implementações de infraestrutura (EF, Redis)

\- \*\*Api\*\* - Controladores e endpoints

\- \*\*Tests\*\* - Testes unitários e de integração



\### Cache com Redis

Implementamos cache distribuído com Redis para:

\- ✅ Melhorar performance em leituras frequentes

\- ✅ Reduzir carga no banco de dados

\- ✅ Garantir resiliência em picos de acesso

\- ✅ Permitir escalabilidade horizontal



\### Middleware Global de Logs

Desenvolvemos middlewares customizados para:

\- \*\*AuditMiddleware\*\* - Rastreabilidade completa de requests

\- \*\*ExceptionMiddleware\*\* - Tratamento centralizado de exceções

\- \*\*Logs estruturados\*\* com Serilog para melhor análise



\### Validações Robustas

\- FluentValidation para validações complexas e customizadas

\- Validações automáticas no pipeline do ASP.NET Core

\- Mensagens de erro claras e contextualizadas



\## 🚀 Como Executar



\### Pré-requisitos

\- Docker

\- Docker Compose



\### Comandos Rápidos

```bash

\# Clone o repositório

git clone https://gitlab.com/cleiton366-group/nepen.git
cd Desafio-NEPEN



\# Subir aplicação

docker-compose up 



\# Parar aplicação

docker-compose down

```



\### Variáveis de Ambiente

```bash

PostgresConnection: "Host=localhost;Database=nepen-dev;Username=postgres;Password=admin;"

Redis\_\_Connection: "localhost:6379"

```



\### Portas dos Serviços

\- \*\*API\*\*: http://localhost:8080

\- \*\*PostgreSQL\*\*: localhost:5432

\- \*\*Redis\*\*: localhost:6379



\## 💾 Scripts de Banco de Dados



\### Backup do Banco

```bash

\# Fazer backup

docker exec -t nepen-postgres pg\_dump -h localhost -U postgres -d nepen-dev > backup\_$(date +%Y%m%d\_%H%M%S).sql



\# Backup rápido

docker exec -t nepen-postgres pg\_dump -h localhost -U postgres -d nepen-dev > backup\_banco.sql

```



\### Restore do Banco

```bash

\# Restaurar from backup file

cat backup\_banco.sql | docker exec -i nepen-postgres psql -h localhost -U postgres -d nepen-dev



\# Com verificação

cat backup\_banco.sql | docker exec -i nepen-postgres psql -h localhost -U postgres -d nepen-dev \&\& echo "✅ Restore completed successfully!"

```



\## 📊 Testes de Performance



\### Execução Local

```bash

\# Navegar para diretório de testes

cd src/Com.Nepen.Tests



\# Executar testes de performance

artillery run teste-performance.yml



\# Ver relatório local

open Artillery/report.json

```



\### Resultados Online

📊 \*\*Dashboard de Performance\*\*: https://app.artillery.io/share/sh\_47dc9224c1f7998bb4952d34c8fcb88a1a27ab88d5bc60cd74c210535ac0287e



\### Configuração dos Testes

Os testes de performance avaliam:

\- ✅ Taxa de requisições por segundo

\- ✅ Tempo de resposta médio

\- ✅ Comportamento sob carga

\- ✅ Estabilidade do sistema



\## 📝 Monitoramento e Logs



\### Rastreabilidade

```bash

\# Ver logs da API

docker logs nepen-api



\# Acessar pasta de logs

./logs

```



\### Estrutura de Logs

Os logs incluem:

\- ✅ Correlation ID para rastreamento

\- ✅ Timestamps precisos

\- ✅ Método HTTP e path

\- ✅ Status codes

\- ✅ Tempo de resposta

\- ✅ Payloads de request/response



\### Health Checks

```bash

\# Verificar saúde da API

curl http://localhost:8080/health



\# Verificar conexão com Redis

docker exec -it nepen-redis redis-cli ping



\# Verificar conexão com PostgreSQL

docker exec -t nepen-postgres psql -h localhost -U postgres -d nepen-dev -c "SELECT NOW();"

```



\## 🔧 Comandos de Desenvolvimento



```bash

\# Aplicar migrations

dotnet ef database update



\# Build da imagem

docker build 

```

