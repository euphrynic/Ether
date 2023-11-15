using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Ether;

public class Bunk
{
    private readonly string[] endPoints = new string[] {
        "https://mainnet.infura.io/v3/e9203d8aad9242dd91bd0ee2f82bc191",
        "https://mainnet.infura.io/v3/67ea47b5c8a240ec8fc6654413e0ddd0",
        "https://mainnet.infura.io/v3/16c00fcc750c4e1a90e547fcab18be43",
    };
    private uint bing;

    public async Task Start(CancellationToken? ct)
    {
        if (ct == null) {
            ct = new CancellationTokenSource().Token;
        }
        var tasks = new Task[endPoints.Length];
        for (var i = 0; i < endPoints.Length; ++i) {
            tasks[i] = Proc(i, (CancellationToken)ct);
        }
        await Task.WhenAll(tasks);
    }

    private async Task Proc(int tid, CancellationToken ct)
    {
        uint cnt = 0;
        for (var retry = 0; retry < 3; ++retry) {
            var web3 = new Web3(endPoints[tid]);
            try {
                while (!ct.IsCancellationRequested) {
                    var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
                    var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                    var account = new Account(privateKey);

                    var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
                    if (balance != null && balance.Value > 0) {
                        Console.WriteLine($"**********\n{tid}.{++bing}.{cnt} {account.Address}\n{privateKey}\n**********");
                        var amount = Web3.Convert.FromWei(balance.Value, Nethereum.Util.UnitConversion.EthUnit.Ether);
                        await File.AppendAllTextAsync("bunk.txt", $"{amount}\t{account.Address}\t{privateKey}\n", ct);
                    }
                    ++cnt;
                    Console.WriteLine($"{tid}.{bing}.{cnt} {account.Address}");
                }
            } catch (Exception ex) {
                Console.Error.WriteLine($"ERR {tid}.{bing}.{cnt} {ex.Message}");
                await Task.Delay(5000, ct);
            }
        }
    }
}

