using Grpc.Core;
using Grpc.Net.Client;
using VotingSystem.Voting;

// Configuração CRÍTICA para conexões gRPC sem SSL
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

Console.WriteLine("--- Cliente Autoridade de Votação (AV) ---");
Console.WriteLine("Conectando a http://ken01.utad.pt:9091...");

try
{
    using var channel = GrpcChannel.ForAddress("http://ken01.utad.pt:9091");
    var client = new VotingService.VotingServiceClient(channel);

    bool running = true;
    while (running)
    {
        Console.WriteLine("\n================ MENU AV ================");
        Console.WriteLine("1 - Consultar Lista de Candidatos");
        Console.WriteLine("2 - Votar");
        Console.WriteLine("3 - Consultar Resultados (Apuramento)");
        Console.WriteLine("0 - Sair");
        Console.Write("Opção: ");
        
        var option = Console.ReadLine();

        try
        {
            switch (option)
            {
                case "1":
                    await ListCandidates(client);
                    break;
                case "2":
                    await Vote(client);
                    break;
                case "3":
                    await ShowResults(client);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
        catch (RpcException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO gRPC] Falha na comunicação com o servidor: {ex.Status.StatusCode}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO] {ex.Message}");
            Console.ResetColor();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro crítico ao inicializar: {ex.Message}");
}

static async Task ListCandidates(VotingService.VotingServiceClient client)
{
    Console.WriteLine("\n--- A obter lista de candidatos... ---");
    var response = await client.GetCandidatesAsync(new GetCandidatesRequest());
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    foreach (var cand in response.Candidates)
    {
        Console.WriteLine($"ID: {cand.Id}\t| Nome: {cand.Name}");
    }
    Console.ResetColor();
}

static async Task Vote(VotingService.VotingServiceClient client)
{
    Console.WriteLine("\n--- Processo de Votação ---");
    Console.Write("Insira a sua Credencial de Voto: ");
    var cred = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(cred))
    {
        Console.WriteLine("Credencial não pode ser vazia.");
        return;
    }

    Console.Write("Insira o ID do Candidato: ");
    if (int.TryParse(Console.ReadLine(), out int candId))
    {
        var response = await client.VoteAsync(new VoteRequest 
        { 
            VotingCredential = cred, 
            CandidateId = candId 
        });

        if (response.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[VOTO CONFIRMADO] {response.Message}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[VOTO RECUSADO] {response.Message}");
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("ID de candidato inválido (deve ser um número).");
    }
    Console.ResetColor();
}

static async Task ShowResults(VotingService.VotingServiceClient client)
{
    Console.WriteLine("\n--- Apuramento Eleitoral Atual ---");
    var response = await client.GetResultsAsync(new GetResultsRequest());
    
    // Ordenar resultados por número de votos (decrescente) para melhor visualização
    var sortedResults = response.Results.OrderByDescending(r => r.Votes);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("{0,-5} | {1,-20} | {2,5}", "ID", "NOME", "VOTOS");
    Console.WriteLine(new string('-', 36));

    foreach (var res in sortedResults)
    {
        Console.WriteLine($"{res.Id,-5} | {res.Name,-20} | {res.Votes,5}");
    }
    Console.ResetColor();
}