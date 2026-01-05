using Grpc.Core;
using Grpc.Net.Client;
using VotingSystem;

// Configuração CRÍTICA para conexões gRPC sem SSL (insecure)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

Console.WriteLine("--- Cliente Autoridade de Registo (AR) ---");
Console.WriteLine("Conectando a http://ken01.utad.pt:9091...");

try 
{
    using var channel = GrpcChannel.ForAddress("http://ken01.utad.pt:9091");
    var client = new VoterRegistrationService.VoterRegistrationServiceClient(channel);

    while (true)
    {
        Console.WriteLine("\n------------------------------------------------");
        Console.Write("Insira o Nº Cartão de Cidadão (ou 's' para sair): ");
        var ccInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(ccInput) || ccInput.ToLower() == "s") break;

        try
        {
            Console.WriteLine("Contactando a AR...");
            var reply = await client.IssueVotingCredentialAsync(new VoterRequest { CitizenCardNumber = ccInput });

            if (reply.IsEligible)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCESSO] Credencial emitida: {reply.VotingCredential}");
                Console.WriteLine("GUARDE ESTA CREDENCIAL PARA VOTAR!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[INFO] Eleitor não elegível ou cartão inválido.");
            }
        }
        catch (RpcException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO gRPC] Status: {ex.Status.StatusCode} - {ex.Status.Detail}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERRO] {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro crítico ao inicializar o canal: {ex.Message}");
}