# Ecommerce Orders API

API RESTful desenvolvida em .NET 8 para gerenciamento de pedidos de um e-commerce, com foco em regras de negócio, organização de código, boas práticas REST, testes automatizados e facilidade de execução.

## Tecnologias

- .NET 8
- ASP.NET Core
- Minimal APIs
- Entity Framework Core
- SQL Server
- Docker
- Docker Compose
- Swagger / OpenAPI
- xUnit

## Arquitetura

A solução está separada em camadas, seguindo princípios de Clean Architecture e separação de responsabilidades.

```text
src/
├── EcommerceOrders.Api
├── EcommerceOrders.Application
├── EcommerceOrders.Domain
└── EcommerceOrders.Infrastructure

tests/
└── EcommerceOrders.UnitTests
```

### EcommerceOrders.Domain

Responsável pelas regras centrais do domínio:

- Entidades
- Status dos pedidos
- Regras de negócio
- Exceções de domínio

### EcommerceOrders.Application

Responsável pelos casos de uso da aplicação:

- DTOs
- Serviços de aplicação
- Interfaces dos repositórios
- Orquestração das operações de pedidos

### EcommerceOrders.Infrastructure

Responsável pelo acesso a dados e persistência:

- Entity Framework Core
- SQL Server
- `AppDbContext`
- Configurações das entidades
- Implementações dos repositórios
- Migrations
- Inicialização e seed do banco

### EcommerceOrders.Api

Responsável pela exposição da aplicação via HTTP:

- Minimal APIs
- Endpoints REST
- Swagger / OpenAPI
- Tratamento global de exceções
- ProblemDetails
- Health Check
- Configuração da aplicação

## Modelo de domínio

O domínio principal é composto por:

- `User`
- `Product`
- `Order`
- `OrderItem`

Um pedido pertence a um comprador e possui um ou mais itens.

O preço utilizado no pedido é armazenado no `OrderItem`, preservando o valor praticado no momento da criação ou alteração do pedido, mesmo que o preço do produto seja modificado posteriormente.

## Regras de negócio

Todo pedido deve possuir:

- Um comprador válido
- Pelo menos um produto
- Quantidade maior que zero para cada item
- Produtos existentes e com preço válido

O cliente não informa o preço dos produtos na criação ou alteração de um pedido. O valor é obtido diretamente do cadastro do produto.

### Status dos pedidos

| Valor | Status |
|------:|--------|
| 1 | Started |
| 2 | Processed |
| 3 | Shipped |
| 4 | Canceled |

### Transições permitidas

```text
Started
  │
  ├── Process ──> Processed ──> Ship ──> Shipped
  │                   │
  │                   └── Cancel ──> Canceled
  │
  └── Cancel ──> Canceled
```

Regras aplicadas:

- Apenas pedidos `Started` podem ser alterados.
- Apenas pedidos `Started` podem ser processados.
- Apenas pedidos `Processed` podem ser enviados.
- Apenas pedidos `Started` ou `Processed` podem ser cancelados.
- Pedidos `Shipped` não podem ser cancelados.
- Pedidos `Canceled` não podem ser alterados.

### Interpretação da regra de alteração

A regra "somente pedidos não processados podem ser alterados" foi interpretada de forma conservadora: apenas pedidos com status `Started` podem sofrer alterações.

Dessa forma, pedidos já enviados ou cancelados permanecem imutáveis.

## Endpoints

Base dos endpoints:

```text
/api/v1/orders
```

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/orders` | Criar pedido |
| GET | `/api/v1/orders` | Listar pedidos |
| GET | `/api/v1/orders/{id}` | Buscar pedido por ID |
| PUT | `/api/v1/orders/{id}` | Alterar pedido |
| DELETE | `/api/v1/orders/{id}` | Excluir pedido |
| POST | `/api/v1/orders/{id}/cancel` | Cancelar pedido |
| POST | `/api/v1/orders/{id}/process` | Processar pedido |
| POST | `/api/v1/orders/{id}/ship` | Enviar pedido |

O versionamento é feito pela rota `/api/v1`.

## Filtro por status

A listagem permite filtrar pedidos pelo status.

Exemplo:

```http
GET /api/v1/orders?status=Started
```

Também podem ser utilizados:

```text
Started
Processed
Shipped
Canceled
```

## Exemplo de criação de pedido

Requisição:

```json
{
  "userId": 1,
  "items": [
    {
      "productId": 1,
      "quantity": 1
    },
    {
      "productId": 2,
      "quantity": 2
    }
  ]
}
```

Exemplo de resposta:

```json
{
  "id": 1,
  "userId": 1,
  "status": 1,
  "createdAt": "2026-09-19T14:15:17Z",
  "updatedAt": null,
  "items": [
    {
      "productId": 1,
      "price": 3500,
      "quantity": 1
    },
    {
      "productId": 2,
      "price": 120,
      "quantity": 2
    }
  ]
}
```

O preço não é enviado pelo cliente. A aplicação consulta o produto no banco e utiliza o preço cadastrado.

## Tratamento de erros

A API possui tratamento global de exceções utilizando `ProblemDetails`.

Principais códigos HTTP utilizados:

| Código | Situação |
|------:|----------|
| 400 | Dados ou parâmetros inválidos |
| 404 | Recurso não encontrado |
| 409 | Violação de regra de negócio |
| 500 | Erro inesperado |

Exemplo de violação de regra de negócio:

```json
{
  "title": "Business rule violation",
  "status": 409,
  "detail": "Only initiated orders can be changed.",
  "instance": "/api/v1/orders/1"
}
```

As exceções de domínio são separadas por responsabilidade, permitindo diferenciar erros de validação, recursos não encontrados e violações de regras de negócio.

## Executando com Docker

### Pré-requisitos

- Docker
- Docker Compose

Na raiz do projeto, execute:

```bash
docker compose up -d --build
```

O Docker Compose inicializa:

- API .NET
- SQL Server
- Banco de dados
- Migrations
- Dados iniciais

### Swagger

```text
http://localhost:8080/swagger
```

### Health Check

```text
http://localhost:8080/health
```

Resposta esperada:

```text
Healthy
```

### SQL Server

O SQL Server fica disponível localmente na porta:

```text
localhost:1433
```

As credenciais definidas no `docker-compose.yml` são destinadas apenas ao ambiente local de desenvolvimento.

### Encerrando os containers

```bash
docker compose down
```

Para encerrar os containers e remover também o volume do banco:

```bash
docker compose down -v
```

## Executando localmente

Caso deseje executar a API fora do Docker, primeiro suba apenas o SQL Server:

```bash
docker compose up -d sqlserver
```

Depois execute:

```bash
dotnet run --project src/EcommerceOrders.Api
```

A porta utilizada no ambiente local pode ser consultada em:

```text
src/EcommerceOrders.Api/Properties/launchSettings.json
```

## Banco de dados

A aplicação utiliza SQL Server através do Entity Framework Core.

As migrations são aplicadas durante a inicialização da aplicação.

Também são inseridos dados iniciais para facilitar os testes.

### Usuários iniciais

| ID | Nome |
|---:|------|
| 1 | João Silva |
| 2 | Maria Souza |

### Produtos iniciais

| ID | Produto | Preço |
|---:|---------|------:|
| 1 | Notebook | R$ 3.500,00 |
| 2 | Mouse | R$ 120,00 |
| 3 | Teclado Mecânico | R$ 250,00 |
| 4 | Monitor | R$ 900,00 |

## Migrations

As migrations do Entity Framework Core estão versionadas no repositório.

Durante a inicialização da API, as migrations pendentes são aplicadas automaticamente.

## Testes

O projeto possui testes unitários cobrindo regras do domínio e serviços da aplicação.

Para executar:

```bash
dotnet test
```

Atualmente a suíte possui 34 testes automatizados.

## Build

Para compilar toda a solução:

```bash
dotnet build
```

## Diferenciais implementados

Além das operações principais de gerenciamento de pedidos, o projeto inclui:

- Arquitetura em camadas
- Minimal APIs
- Versionamento por rota (`/api/v1`)
- Filtro por status
- Swagger / OpenAPI
- Docker
- Docker Compose
- Testes unitários
- Health Check
- Tratamento global de exceções
- ProblemDetails
- Entity Framework Core Migrations
- Seed de dados
- Endpoints de processamento e envio de pedidos

## Decisões técnicas

### .NET 8

Foi utilizado .NET 8 por ser uma versão LTS da plataforma.

### Minimal APIs

Minimal APIs foram utilizadas por serem adequadas ao tamanho e ao escopo da aplicação, mantendo os endpoints enxutos e de fácil leitura.

### Snapshot do preço

O preço do produto é copiado para o `OrderItem` no momento da criação ou alteração do pedido.

Isso preserva o valor utilizado no pedido mesmo que o preço atual do produto seja alterado posteriormente.

### Exceções específicas

Foram utilizadas categorias diferentes de exceção para representar:

- Validações de domínio
- Recursos não encontrados
- Violações de regras de negócio

O handler global converte essas exceções para respostas HTTP adequadas.

### Docker Compose

A aplicação e o SQL Server podem ser inicializados juntos por meio do Docker Compose.

Dentro da rede do Docker, a API acessa o banco através do nome do serviço `sqlserver`, evitando dependência de uma instalação local do SQL Server.

## Estrutura geral

```text
ecommerce-orders-api/
│
├── src/
│   ├── EcommerceOrders.Api/
│   ├── EcommerceOrders.Application/
│   ├── EcommerceOrders.Domain/
│   └── EcommerceOrders.Infrastructure/
│
├── tests/
│   └── EcommerceOrders.UnitTests/
│
├── .dockerignore
├── .gitignore
├── Dockerfile
├── docker-compose.yml
├── EcommerceOrders.sln
└── README.md
```
