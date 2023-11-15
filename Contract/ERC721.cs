using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Twin.Contract;

public class ERC721
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


    public async Task<TransactionReceipt> Approve(string to, BigInteger tokenId)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new ApproveFunction { To = to, TokenId = tokenId });
    }
    public async Task<BigInteger> BalanceOf(string owner)
    {
        return await handler.QueryAsync<BalanceOfFunction, BigInteger>(
            new BalanceOfFunction { Owner = owner });
    }
    public async Task<string> GetApproved(BigInteger tokenId)
    {
        return await handler.QueryAsync<GetApprovedFunction, string>(
            new GetApprovedFunction { TokenId = tokenId });
    }
    public async Task<bool> IsApprovedForAll(string owner, string oper)
    {
        return await handler.QueryAsync<IsApprovedForAllFunction, bool>(
            new IsApprovedForAllFunction { Owner = owner, Operator = oper });
    }
    public async Task<string> OwnerOf(BigInteger tokenId)
    {
        return await handler.QueryAsync<OwnerOfFunction, string>(
            new OwnerOfFunction { TokenId = tokenId });
    }
    public async Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger tokenId)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SafeTransferFromFunction { From = from, To = to, TokenId = tokenId });
    }
    public async Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger tokenId, byte[] data)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SafeTransferFromWithDataFunction { From = from, To = to, TokenId = tokenId, Data = data });
    }
    public async Task<TransactionReceipt> SetApprovalForAll(string oper, bool approved)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new SetApprovalForAllFunction { Operator = oper, Approved = approved });
    }
    public async Task<TransactionReceipt> TransferFrom(string from, string to, BigInteger tokenId)
    {
        return await handler.SendRequestAndWaitForReceiptAsync(
            new TransferFromFunction { From = from, To = to, TokenId = tokenId });
    }




    [Function("approve")]
    private class ApproveFunction : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "tokenId", 2)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Function("balanceOf", "uint256")]
    private class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; } = null!;
    }

    [Function("getApproved", "address")]
    private class GetApprovedFunction : FunctionMessage
    {
        [Parameter("uint256", "tokenId", 1)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Function("isApprovedForAll", "bool")]
    private class IsApprovedForAllFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; } = null!;
        [Parameter("address", "operator", 2)]
        public virtual string Operator { get; set; } = null!;
    }

    [Function("ownerOf", "address")]
    private class OwnerOfFunction : FunctionMessage
    {
        [Parameter("uint256", "tokenId", 1)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Function("safeTransferFrom")]
    private class SafeTransferFromFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "tokenId", 3)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Function("safeTransferFrom")]
    private class SafeTransferFromWithDataFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "tokenId", 3)]
        public virtual BigInteger TokenId { get; set; }
        [Parameter("bytes", "data", 4)]
        public virtual byte[] Data { get; set; } = null!;
    }

    [Function("setApprovalForAll")]
    private class SetApprovalForAllFunction : FunctionMessage
    {
        [Parameter("address", "operator", 1)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("bool", "_approved", 2)]
        public virtual bool Approved { get; set; }
    }

    [Function("transferFrom")]
    private class TransferFromFunction : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "tokenId", 3)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Event("Approval")]
    private class ApprovalEventDTO : IEventDTO
    {
        [Parameter("address", "owner", 1, true)]
        public virtual string Owner { get; set; } = null!;
        [Parameter("address", "approved", 2, true)]
        public virtual string Approved { get; set; } = null!;
        [Parameter("uint256", "tokenId", 3, true)]
        public virtual BigInteger TokenId { get; set; }
    }

    [Event("ApprovalForAll")]
    private class ApprovalForAllEventDTO : IEventDTO
    {
        [Parameter("address", "owner", 1, true)]
        public virtual string Owner { get; set; } = null!;
        [Parameter("address", "operator", 2, true)]
        public virtual string Operator { get; set; } = null!;
        [Parameter("bool", "approved", 3, false)]
        public virtual bool Approved { get; set; }
    }

    [Event("Transfer")]
    private class TransferEventDTO : IEventDTO
    {
        [Parameter("address", "from", 1, true)]
        public virtual string From { get; set; } = null!;
        [Parameter("address", "to", 2, true)]
        public virtual string To { get; set; } = null!;
        [Parameter("uint256", "tokenId", 3, true)]
        public virtual BigInteger TokenId { get; set; }
    }



    [FunctionOutput]
    public class BalanceOfOutputDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "balance", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    [FunctionOutput]
    private class GetApprovedOutputDTO : IFunctionOutputDTO
    {
        [Parameter("address", "operator", 1)]
        public virtual string Operator { get; set; } = null!;
    }

    [FunctionOutput]
    private class IsApprovedForAllOutputDTO : IFunctionOutputDTO
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    [FunctionOutput]
    private class OwnerOfOutputDTO : IFunctionOutputDTO
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; } = null!;
    }

}

