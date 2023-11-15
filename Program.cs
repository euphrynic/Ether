using System.Numerics;
using Nethereum.Web3;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Extensions;

namespace Ether;

public static class Program
{
    private static readonly CancellationTokenSource cts = new();
    private static readonly Random rnd = new();
    private static readonly string[] endPoints = new string[] {
        "https://mainnet.infura.io/v3/e9203d8aad9242dd91bd0ee2f82bc191",
        "https://mainnet.infura.io/v3/67ea47b5c8a240ec8fc6654413e0ddd0",
        "https://mainnet.infura.io/v3/16c00fcc750c4e1a90e547fcab18be43",
        //"https://kovan.poa.network".
    };
    private static int pointIdx = 0;
    private static decimal minGas = 15m;
    private const string contractAddress = "0xc18360217d8f7ab5e7c516566761ea12ce7f9d72";
    private const string logFile = "log.txt";
    private const string logFile0 = "log0.txt";

    public static async Task Main(string[] args)
    {
        var path = Directory.GetCurrentDirectory();
        if (path.EndsWith("/bin/Debug/net6.0")) {
            path = path[..^17];
            Directory.SetCurrentDirectory(path);
        }

        Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) => {
            args.Cancel = true;
            cts.Cancel();
        });

        await new Bunk().Start(cts.Token);

        //try {
        //    if (File.Exists(logFile)) {
        //        if (File.Exists(logFile0)) {
        //            File.Delete(logFile0);
        //        }
        //        File.Move(logFile, logFile0);
        //    }
        //} catch { }

        //if (args.Length > 0) {
        //    _ = decimal.TryParse(args[0], out minGas);
        //}

        //pointIdx = rnd.Next(endPoints.Length);
        //if (await WaitMinGas()) {
        //    await Proc();
        //} else {
        //    await AppendLog("查询gas价格时发生错误");
        //}

        //Console.WriteLine();
    }

    private static async Task<bool> WaitMinGas(Web3? web3 = null)
    {
        var rlt = false;
        var unitConversion = new UnitConversion();
        var tryCnt = 3;
        web3 ??= new Web3(endPoints[pointIdx]);
        while (!rlt && !cts.IsCancellationRequested && tryCnt > 0) {
            try {
                var wei = await web3.Eth.GasPrice.SendRequestAsync();
                var gwei = unitConversion.FromWei(wei, UnitConversion.EthUnit.Gwei);
                //gwei = decimal.Round(gwei, 2);
                await AppendLog($"{DateTime.Now:yyyyMMdd_HHmmss} {gwei}");
                if (gwei <= minGas) {
                    rlt = true;
                }
            } catch {
                --tryCnt; if (++pointIdx >= endPoints.Length) { pointIdx = 0; }
                Console.Write("*");
                web3 = new Web3(endPoints[pointIdx]);
            }
            try {
                await Task.Delay(1000 * 30, cts.Token);
            } catch {}
        }
        
        return rlt;
    }

    private static async Task Proc()
    {
        var privs = await File.ReadAllLinesAsync("priv.txt");
        for (var idx = 0; !cts.IsCancellationRequested && idx < privs.Length; ++idx) {
            try {
                var account = new Account(privs[idx], Nethereum.Signer.Chain.MainNet);
                var web3 = new Web3(account, endPoints[pointIdx]);

                var contractHandler = web3.Eth.GetContractHandler(contractAddress);

                var msg = new DelegateFunction { Delegatee = account.Address };
                var rcp = await contractHandler.SendRequestAndWaitForReceiptAsync(msg);
                await AppendLog($"{(idx + 1):000} {rcp.Status} {rcp.GasUsed} {rcp.EffectiveGasPrice} {rcp.TransactionHash}");

                //var msg = new DelegateFunction { Gas = 1000000, Delegatee = account.Address };
                //msg.Gas = await contractHandler.EstimateGasAsync(msg);
                //Console.WriteLine($"estimate {msg.Gas} {msg.GetCallData().ToHexStream()}");
                //var rcp = await contractHandler.SendRequestAndWaitForReceiptAsync(msg);
                //Console.WriteLine($"{(idx + 1):0000} {rcp.Status} {rcp.GasUsed} {rcp.TransactionHash}");
                await WaitMinGas(web3);
            } catch (Exception ex) {
                await AppendLog($"{(idx + 1):000} {ex.Message}");
                // try again
                if (++pointIdx >= endPoints.Length) { pointIdx = 0; }
                try {
                    var account = new Account(privs[idx], Nethereum.Signer.Chain.MainNet);
                    var web3 = new Web3(account, endPoints[pointIdx]);
                    var contractHandler = web3.Eth.GetContractHandler(contractAddress);
                    var msg = new DelegateFunction { Delegatee = account.Address };
                    var rcp = await contractHandler.SendRequestAndWaitForReceiptAsync(msg);
                    await AppendLog($"{(idx + 1):000} {rcp.Status} {rcp.GasUsed} {rcp.EffectiveGasPrice} {rcp.TransactionHash}");
                    await WaitMinGas(web3);
                } catch (Exception exx) {
                    await AppendLog($"{(idx + 1):000} {exx.Message}");
                }
            }
        }
    }

    private static async Task AppendLog(string log)
    {
        Console.WriteLine(log);
        try {
            await File.AppendAllLinesAsync(logFile, new string[] { log });
        } catch { }
    }


    [Function("delegate")]
    protected class DelegateFunction : FunctionMessage
    {
        [Parameter("address", "delegatee", 1)]
        public virtual string Delegatee { get; set; } = "";
    }




    //safeTransferFrom(address _from, address _to, uint256 _id, uint256 _value, bytes _data)
    //tid:  108379438876408987843446820442003143922756991564627423690903362373873737662465
    //caddr: 0x88b48f654c30e99bc2e4a1559b4dcf1ad93fa656
    //from:0xEF9c96aCe25Ff22056a93aaf612106d9Ff5C9E04
    //to:0x15C26BDa3298cF17706E7F016da443B4eb57c3A5
}


