using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts;

namespace Twin.Contract;

//https://remix.ethereum.org/
//http://playground.nethereum.com/
//https://github.com/OpenZeppelin/openzeppelin-contracts
//https://docs.openzeppelin.com/contracts/4.x/wizard

public class ERC20
{
    public string AccessPoint { get; set; } = "";
    public string ContractAddress { get; set; } = "";
    private Web3 web3 = null!;
    private Nethereum.Contracts.ContractHandlers.ContractHandler handler = null!;

    public void Connect()
    {
        web3 = new Web3(AccessPoint);
        handler = web3.Eth.GetContractHandler(ContractAddress);
    }
    public void Connect(string AccessPoint, string ContractAddress)
    {
        this.AccessPoint = AccessPoint;
        this.ContractAddress = ContractAddress;
        web3 = new Web3(AccessPoint);
        handler = web3.Eth.GetContractHandler(ContractAddress);
    }




    //public async Task<string> TokenName()
    //{
    //    var web3 = new Web3(AccessPoint);
    //    var contractHandler = web3.Eth.GetContractHandler(ContractAddress);
    //    var name = await contractHandler.QueryAsync<NameFunction, string>();
    //    Console.WriteLine("Token Name: " + name);
    //    return name;
    //}
    //public async Task<string> TokenSymbol()
    //{
    //    var web3 = new Web3(AccessPoint);
    //    var contractHandler = web3.Eth.GetContractHandler(contractAddress);
    //    var symbol = await contractHandler.QueryAsync<SymbolFunction, string>();
    //    Console.WriteLine("Symbol Name: " + symbol);
    //    return symbol;
    //}
    //public static async Task QueryBalance(int fromIdx, int toIdx)
    //{
    //    --fromIdx;
    //    var pubs = Ether.Pubs.Skip(fromIdx).Take(toIdx - fromIdx).ToArray();
    //    var idx = fromIdx;
    //    var web3 = new Web3(Ether.chainUrl);
    //    var contractHandler = web3.Eth.GetContractHandler(contractAddress ?? contractAddress0);

    //    foreach (var pub in pubs) {
    //        try {
    //            var balanceOfFunction = new BalanceOfFunction { TokenOwner = pub };
    //            var balance = await contractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction);
    //            var amount = Web3.Convert.FromWei(balance);
    //            Console.WriteLine($"{(idx + 1):0000} {pub} {amount}");
    //        } catch (Exception ex) {
    //            Console.WriteLine($"{(idx + 1):0000} {ex.Message}");
    //        }
    //        ++idx;
    //    }
    //}

    //protected static readonly string contractAddress0 = "0x702e63b4d496f414c49b61a3ad44a83ab2a538a1";
    //public static string? contractAddress;




    public async Task<BigInteger> Allowance(string owner, string del)
    {
        return await handler.QueryAsync<AllowanceFunction, BigInteger>(
            new AllowanceFunction { Owner = owner, Delegate = del });
    }
    public async Task<TransactionReceipt> Approve(string del, BigInteger num)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new ApproveFunction { Delegate = del, NumTokens = num });
    }
    public async Task<BigInteger> BalanceOf(string owner)
    {
        return await handler.QueryAsync<BalanceOfFunction, BigInteger>(
            new BalanceOfFunction { TokenOwner = owner });
    }
    public async Task<byte> Decimals()
    {
        return await handler.QueryAsync<DecimalsFunction, byte>();
    }
    public async Task<string> Name()
    {
        return await handler.QueryAsync<NameFunction, string>();
    }
    public async Task<string> Symbol()
    {
        return await handler.QueryAsync<SymbolFunction, string>();
    }
    public async Task<BigInteger> TotalSupply()
    {
        return await handler.QueryAsync<TotalSupplyFunction, BigInteger>();
    }
    public async Task<TransactionReceipt> Transfer(string recv, BigInteger num)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new TransferFunction { Receiver = recv, NumTokens = num });
    }
    public async Task<TransactionReceipt> TransferFrom(string owner, string buyer, BigInteger num)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new TransferFromFunction { Owner = owner, Buyer = buyer, NumTokens = num });
    }



    [Function("allowance", "uint256")]
    protected class AllowanceFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; } = "";
        [Parameter("address", "delegate", 2)]
        public virtual string Delegate { get; set; } = "";
    }

    [Function("approve", "bool")]
    protected class ApproveFunction : FunctionMessage
    {
        [Parameter("address", "delegate", 1)]
        public virtual string Delegate { get; set; } = "";
        [Parameter("uint256", "numTokens", 2)]
        public virtual BigInteger NumTokens { get; set; }
    }

    [Function("balanceOf", "uint256")]
    protected class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "tokenOwner", 1)]
        public virtual string TokenOwner { get; set; } = "";
    }

    [Function("decimals", "uint8")]
    protected class DecimalsFunction : FunctionMessage
    {

    }

    [Function("name", "string")]
    protected class NameFunction : FunctionMessage
    {

    }

    [Function("symbol", "string")]
    protected class SymbolFunction : FunctionMessage
    {

    }

    [Function("totalSupply", "uint256")]
    protected class TotalSupplyFunction : FunctionMessage
    {

    }

    [Function("transfer", "bool")]
    protected class TransferFunction : FunctionMessage
    {
        [Parameter("address", "receiver", 1)]
        public virtual string Receiver { get; set; } = "";
        [Parameter("uint256", "numTokens", 2)]
        public virtual BigInteger NumTokens { get; set; }
    }

    [Function("transferFrom", "bool")]
    protected class TransferFromFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; } = "";
        [Parameter("address", "buyer", 2)]
        public virtual string Buyer { get; set; } = "";
        [Parameter("uint256", "numTokens", 3)]
        public virtual BigInteger NumTokens { get; set; }
    }

    [Event("Approval")]
    protected class ApprovalEventDTO : IEventDTO
    {
        [Parameter("address", "tokenOwner", 1, true)]
        public virtual string TokenOwner { get; set; } = "";
        [Parameter("address", "spender", 2, true)]
        public virtual string Spender { get; set; } = "";
        [Parameter("uint256", "tokens", 3, false)]
        public virtual BigInteger Tokens { get; set; }
    }

    [Event("Transfer")]
    protected class TransferEventDTO : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public virtual string From { get; set; } = "";
        [Parameter("address", "to", 2, true)]
        public virtual string To { get; set; } = "";
        [Parameter("uint256", "tokens", 3, false)]
        public virtual BigInteger Tokens { get; set; }
    }

    [FunctionOutput]
    protected class AllowanceOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    [FunctionOutput]
    protected class BalanceOfOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    [FunctionOutput]
    protected class DecimalsOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint8", "", 1)]
        public virtual byte ReturnValue1 { get; set; }
    }

    [FunctionOutput]
    protected class NameOutputDTO : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; } = "";
    }

    [FunctionOutput]
    protected class SymbolOutputDTO : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; } = "";
    }

    [FunctionOutput]
    protected class TotalSupplyOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }
}

