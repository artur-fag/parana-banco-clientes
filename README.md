# Paraná Banco | Microsserviço de Clientes

API REST para cadastro e consulta de clientes no desafio técnico Backend do Paraná Banco. O cadastro inicia o fluxo assíncrono de concessão de crédito por meio do evento `ClienteCadastrado`.

## Visão geral

```text
POST /api/v1/clientes
        |
        v
  Clientes API (.NET 10)
        |
        v
SQL Server: cliente + mensagem Outbox (mesma transação)
        |
        v
Worker Outbox -> RabbitMQ -> Microsserviço Propostas -> Microsserviço Cartões
```

A comunicação entre serviços é assíncrona. Propostas calcula o score e Cartões emite os cartões conforme a aprovação; essas ações **não** são disparadas por chamadas HTTP adicionais ao Clientes.

## Tecnologias e organização

- .NET 10, ASP.NET Core Web API e C#.
- Entity Framework Core e SQL Server 2022.
- RabbitMQ 4, com publicação assíncrona via padrão **Transactional Outbox**.
- xUnit para testes automatizados.
- Organização em projetos `Domain`, `Application`, `Infrastructure` e `API`, com projetos de testes separados.

## Pré-requisitos

- SDK do .NET 10.
- Docker Desktop com Docker Compose habilitado.
- PowerShell ou outro terminal.

## Infraestrutura local

O arquivo `compose.yaml` deste repositório sobe dois contêineres:

| Serviço | Endereço local | Observação |
| --- | --- | --- |
| SQL Server | `localhost,1434` | Porta interna do contêiner: 1433 |
| RabbitMQ (AMQP) | `localhost:5672` | Mensageria |
| RabbitMQ Management | `http://localhost:15672` | Interface administrativa |

Crie um arquivo `.env` **local**, na raiz do repositório, com os valores exigidos pelo Compose:

```dotenv
MSSQL_SA_PASSWORD=<senha-forte-local>
RABBITMQ_USER=<usuario-local>
RABBITMQ_PASSWORD=<senha-local>
```

**Nunca publique o `.env`, senhas ou strings de conexão no GitHub.** Confira se `.env` está ignorado pelo Git.

Inicie a infraestrutura:

```powershell
docker compose up -d
docker compose ps
```

> Os três microserviços podem utilizar a mesma instância local de SQL Server e RabbitMQ, mantendo bancos de dados separados.

## Configuração da API

As configurações sensíveis da API devem ser fornecidas por **.NET User Secrets** durante o desenvolvimento, ou por variáveis de ambiente em outros ambientes. Consulte `src/Clientes.Infrastructure/DependencyInjection/` para confirmar os nomes exatos das chaves de configuração exigidas pela implementação, especialmente a conexão com o banco e as credenciais do RabbitMQ.

Para iniciar o armazenamento local de segredos, caso ainda não esteja habilitado:

```powershell
dotnet user-secrets init --project src/Clientes.API
```

Configure as chaves conforme os nomes efetivamente utilizados no código, sem registrar os valores no repositório.

## Banco de dados e migrations

As migrations ficam em `src/Clientes.Infrastructure/Persistence/Migrations/`. Com a conexão configurada e o SQL Server iniciado, aplique as migrations usando o projeto de infraestrutura e a API como startup:

```powershell
dotnet ef database update --project src/Clientes.Infrastructure --startup-project src/Clientes.API
```

Se `dotnet ef` não estiver disponível, instale ou restaure a ferramenta correspondente à versão do EF Core utilizada no projeto.

## Executar a API

```powershell
dotnet restore Clientes.slnx
dotnet build Clientes.slnx
dotnet run --project src/Clientes.API --urls http://localhost:5050
```

Em ambiente `Development`, o projeto expõe:

- Swagger UI: `http://localhost:5050/swagger`
- Documento OpenAPI: `http://localhost:5050/openapi/v1.json`

## Endpoints

| Método | Rota | Objetivo |
| --- | --- | --- |
| `POST` | `/api/v1/clientes` | Cadastrar um cliente e iniciar o fluxo de crédito |
| `GET` | `/api/v1/clientes/{id}` | Consultar um cliente pelo identificador |

O cadastro valida CPF, e-mail e dados obrigatórios. Utilize o Swagger UI para verificar o contrato JSON exato do endpoint e enviar uma requisição de exemplo.

## Como acompanhar o fluxo completo

1. Cadastre um cliente com `POST /api/v1/clientes` e guarde o `clienteId` retornado.
2. O Clientes grava a mensagem `ClienteCadastrado` na Outbox; o worker a publica no RabbitMQ.
3. Consulte `GET http://localhost:5150/api/v1/propostas/{clienteId}` para acompanhar a proposta gerada.
4. Consulte `GET http://localhost:5250/api/v1/cartoes/{clienteId}` para verificar os cartões emitidos, se aprovados.
5. Como o fluxo é assíncrono, aguarde alguns segundos antes de consultar os serviços seguintes.

Os serviços Propostas e Cartões precisam estar executando e conectados à mesma infraestrutura de mensageria.

## Testes

```powershell
dotnet test Clientes.slnx
```

O repositório possui projetos de testes unitários e de integração. Execute-os novamente antes da entrega; não considere resultados anteriores como garantia do estado atual.

## Decisões técnicas

- **Separação de responsabilidades:** Clientes gerencia cadastro; Propostas analisa crédito; Cartões emite cartões.
- **Transactional Outbox:** evita perder a intenção de publicação quando o cadastro é persistido, mas o broker está temporariamente indisponível.
- **Consistência eventual:** o resultado da proposta e dos cartões não precisa estar disponível imediatamente após o cadastro.
- **Configuração segura:** segredos fora do controle de versão.

## Repositórios relacionados

- [Clientes](https://github.com/artur-fag/parana-banco-clientes)
- [Propostas](https://github.com/artur-fag/parana-banco-propostas)
- [Cartões](https://github.com/artur-fag/parana-banco-cartoes)

## Fluxograma da solução

O desafio também exige um fluxograma dos três microserviços e do hub de mensageria. Inclua o arquivo `Fluxograma_Parana_Banco.drawio` na documentação da entrega ou referencie o diagrama publicado em um dos repositórios.
