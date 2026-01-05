## Estrutura

A solução está dividida em dois clientes distintos para garantir a separação de responsabilidades (Registo vs Votação):

RegistoClient: Aplicação cliente da Autoridade de Registo (AR). Responsável por identificar o eleitor e obter a credencial anónima.

VotacaoClient: Aplicação cliente da Autoridade de Votação (AV). Responsável por submeter o voto utilizando a credencial.

Protos: Contém os ficheiros `.proto` (`voter.proto` e `voting.proto`) que definem os contratos de serviço gRPC.

## Pré-requisitos

.NET SDK 6.0, 7.0 ou 8.0(https://dotnet.microsoft.com/download)

Ligação à Internet (para aceder ao servidor remoto `ken01.utad.pt`)

## Configuração e Instalação

Antes de executar, é necessário restaurar as dependências e compilar o projeto:

bash

# Na raiz do projeto

dotnet build


## Guia de Utilização

O sistema funciona em dois passos lógicos. Deves executar as aplicações na seguinte ordem:

Passo 1: Obter Credencial (Autoridade de Registo)

1.  Aceda à pasta do cliente de registo e execute:

bash

cd RegistoClient

dotnet run

2.  Introduza um número de Cartão de Cidadão (ex: `123456789`).

3.  Se for elegível, o sistema devolverá uma **Credencial de Voto** (ex: `CRED-ABC-123`).

4.  **Copie essa credencial.**

Passo 2: Votar (Autoridade de Votação)

1.  Aceda à pasta do cliente de votação e execute:

bash

cd VotacaoClient

dotnet run

2.  No menu, selecione a opção 1 para ver os candidatos e memorizar um ID.

3.  Selecione a opção 2 - Votar.

4.  Cole a Credencial de Voto obtida no Passo 1.

5.  Insira o ID do candidato.

## Detalhes Técnicos

Endpoint do Servidor: https://ken01.utad.pt:9091

Segurança SSL: O código está configurado para aceitar certificados não confiáveis 

Mockserver: O comportamento do servidor é simulado (70% das credenciais são válidas, 30% inválidas).


