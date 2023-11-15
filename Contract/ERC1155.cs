using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Twin.Contract;

public class ERC1155
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



    public async Task<BigInteger> BalanceOf(string account, BigInteger id)
    {
        return await handler.QueryAsync<BalanceOfFunction, BigInteger>(
            new BalanceOfFunction { Account = account, Id = id });
    }

    public async Task<List<BigInteger>> BalanceOfBatch(List<string> accounts, List<BigInteger> ids)
    {
        return await handler.QueryAsync<BalanceOfBatchFunction, List<BigInteger>>(
            new BalanceOfBatchFunction { Accounts = accounts, Ids = ids });
    }

    public async Task<bool> IsApprovedForAll(string account, string oper)
    {
        return await handler.QueryAsync<IsApprovedForAllFunction, bool>(
            new IsApprovedForAllFunction { Account = account, Operator = oper });
    }

    public async Task<TransactionReceipt> SafeBatchTransferFrom(string from, string to, List<BigInteger> ids, List<BigInteger> amounts, byte[] data)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SafeBatchTransferFromFunction { From = from, To = to, Ids = ids, Amounts = amounts, Data = data });
    }

    public async Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger id, BigInteger amount, byte[] data)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SafeTransferFromFunction { From = from, To = to, Id = id, Amount = amount, Data = data });
    }

    public async Task<TransactionReceipt> SetApprovalForAll(string oper, bool approved)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SetApprovalForAllFunction { Operator = oper, Approved = approved });
    }

    public async Task<bool> SupportsInterface(byte[] interfaceId)
    {
        return await handler.QueryAsync<SupportsInterfaceFunction, bool>(
            new SupportsInterfaceFunction { InterfaceId = interfaceId });
    }






    [Function("balanceOf", "uint256")]
    private class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; } = null!;
        [Parameter("uint256", "id", 2)]
        public virtual BigInteger Id { get; set; }
    }

    [Function("balanceOfBatch", "uint256[]")]
    private class BalanceOfBatchFunction : FunctionMessage
    {
        [Parameter("address[]", "accounts", 1)]
        public virtual List<string> Accounts { get; set; } = null!;
        [Parameter("uint256[]", "ids", 2)]
        public virtual List<BigInteger> Ids { get; set; } = null!;
    }

    [Function("isApprovedForAll", "bool")]
    private class IsApprovedForAllFunction : FunctionMessage
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; } = null!;
        [Parameter("address", "operator", 2)]
        public virtual string Operator { get; set; } = null!;
    }

    [Function("safeBatchTransferFrom")]
    private class SafeBatchTransferFromFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256[]", "ids", 3)]
        public virtual List<BigInteger> Ids { get; set; } = null!;
        [Parameter("uint256[]", "amounts", 4)]
        public virtual List<BigInteger> Amounts { get; set; } = null!;
        [Parameter("bytes", "data", 5)]
        public virtual byte[] Data { get; set; } = null!;
    }

    [Function("safeTransferFrom")]
    private class SafeTransferFromFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "id", 3)]
        public virtual BigInteger Id { get; set; }
        [Parameter("uint256", "amount", 4)]
        public virtual BigInteger Amount { get; set; }
        [Parameter("bytes", "data", 5)]
        public virtual byte[] Data { get; set; } = null!;
    }

    [Function("setApprovalForAll")]
    private class SetApprovalForAllFunction : FunctionMessage
    {
        [Parameter("address", "operator", 1)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("bool", "approved", 2)]
        public virtual bool Approved { get; set; }
    }

    [Function("supportsInterface", "bool")]
    private class SupportsInterfaceFunction : FunctionMessage
    {
        [Parameter("bytes4", "interfaceId", 1)]
        public virtual byte[] InterfaceId { get; set; } = null!;
    }

    [Event("ApprovalForAll")]
    private class ApprovalForAllEventDTO : IEventDTO
    {
        [Parameter("address", "account", 1, true)]
        public virtual string Account { get; set; } = null!;
        [Parameter("address", "operator", 2, true)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("bool", "approved", 3, false)]
        public virtual bool Approved { get; set; }
    }

    [Event("TransferBatch")]
    private class TransferBatchEventDTO : IEventDTO
    {
        [Parameter("address", "operator", 1, true)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("address", "from", 2, true)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 3, true)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256[]", "ids", 4, false)]
        public virtual List<BigInteger> Ids { get; set; } = null!;
        [Parameter("uint256[]", "values", 5, false)]
        public virtual List<BigInteger> Values { get; set; } = null!;
    }

    [Event("TransferSingle")]
    private class TransferSingleEventDTO : IEventDTO
    {
        [Parameter("address", "operator", 1, true)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("address", "from", 2, true)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 3, true)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "id", 4, false)]
        public virtual BigInteger Id { get; set; }
        [Parameter("uint256", "value", 5, false)]
        public virtual BigInteger Value { get; set; }
    }

    [Event("URI")]
    private class URIEventDTO : IEventDTO
    {
        [Parameter("string", "value", 1, false)]
        public virtual string Value { get; set; } = null!;
        [Parameter("uint256", "id", 2, true)]
        public virtual BigInteger Id { get; set; }
    }

    [FunctionOutput]
    private class BalanceOfOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    [FunctionOutput]
    private class BalanceOfBatchOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256[]", "", 1)]
        public virtual List<BigInteger> ReturnValue1 { get; set; } = null!;
    }

    [FunctionOutput]
    private class IsApprovedForAllOutputDTO : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    [FunctionOutput]
    private class SupportsInterfaceOutputDTO : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

}

