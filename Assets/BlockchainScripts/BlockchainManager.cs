using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using UnityEngine.UI;
using System.Numerics;
using System;
using System.Data;
using UnityEngine.SceneManagement;

public class BlockchainManager : MonoBehaviour
{
    public string Address { get; private set; }
    public static BigInteger ChainId = 1001;

    public Button nftButton;
    public Button playButton;
    public Button goldButton;
    public Button gemButton;

    public TMP_Text claimNFTNoticeText;
    public TMP_Text buyingStatusText;

    public TMP_Text goldBoughtText;
    public TMP_Text gemBoughtText;

    string NFTAddressSmartContract = "0x7b44A6a631aC29Eac1aee0194137B659cd3074B9";

    string receiverAddress = "0xA24d7ECD79B25CE6C66f1Db9e06b66Bd11632E00";
    private bool hasAddGold = false;
    private bool hasAddGem = false;

    int goldValue = 10000;
    int gemValue = 100;

    int itemPrice = 1;

    private void Start()
    {
        nftButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        goldButton.gameObject.SetActive(false);
        gemButton.gameObject.SetActive(false);
        claimNFTNoticeText.gameObject.SetActive(false);
        buyingStatusText.gameObject.SetActive(false);
        Login();
    }

    public async void Login()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        Address = await wallet.GetAddress();
        var contract = await ThirdwebManager.Instance.GetContract(
          NFTAddressSmartContract,
          ChainId
       );
        var nftList = await contract.ERC721_BalanceOf(Address);
        Debug.Log(nftList);

        // Kiểm tra số lượng NFT
        if (nftList >= 1)
        {
            playButton.gameObject.SetActive(true);
            nftButton.gameObject.SetActive(false);
            goldButton.gameObject.SetActive(true);
            gemButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("b");
            claimNFTNoticeText.gameObject.SetActive(true);
            nftButton.gameObject.SetActive(true);
        }
    }

    public async void ClaimNFTPass()
    {
        UpdateStatus("Claiming...");
        nftButton.interactable = false;
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        Address = await wallet.GetAddress();
        var contract = await ThirdwebManager.Instance.GetContract(
          NFTAddressSmartContract,
          ChainId
       );

        var nftList1 = await contract.ERC721_BalanceOf(Address);
        Debug.Log(nftList1);
        try
        {
            var result = await contract.DropERC721_Claim(wallet, Address, 1);

            var nftList2 = await contract.ERC721_BalanceOf(Address);
            Debug.Log(nftList2);

            UpdateStatus("Claimed NFT Pass!");
            nftButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            goldButton.gameObject.SetActive(true);
            gemButton.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            var nftList3 = await contract.ERC721_BalanceOf(Address);
            Debug.Log(nftList3);

            if (nftList3 > nftList1)
            {
                Debug.Log("Claim NFT Successfully");
                UpdateStatus("Claimed NFT Pass!");
            }

            Debug.LogError($"An error occurred: {ex.Message}");
            // Xử lý lỗi tại đây, ví dụ hiển thị thông báo lỗi cho người
            nftButton.interactable = true;
            nftButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            goldButton.gameObject.SetActive(false);
            gemButton.gameObject.SetActive(false);

            nftList3 = await contract.ERC721_BalanceOf(Address);
            Debug.Log(nftList3);

            if (nftList3 > nftList1)
            {
                Debug.Log("Claim NFT Successfully");
                UpdateStatus("Claimed NFT Pass!");
            }
        }
    }

    private BigInteger ConvertToWei(decimal amount)
    {
        // 1 token = 10^18 Wei
        BigInteger wei = new BigInteger(amount * (decimal)Math.Pow(10, 18));
        return wei;
    }

    private void HideAllButtons()
    {
        nftButton.interactable = false;
        playButton.interactable = false;
        goldButton.interactable = false;
        gemButton.interactable = false;
    }

    private void ShowAllButtons()
    {
        nftButton.interactable = true;
        playButton.interactable = true;
        goldButton.interactable = true;
        gemButton.interactable = true;
    }

    private void UpdateStatus(string messageShow)
    {
        buyingStatusText.text = messageShow;
        buyingStatusText.gameObject.SetActive(true);
    }

    public async void CheckWalletBalance(string balanceEth1, int indexValue)
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);

        Debug.Log("balanceEth: " + balanceEth);

        if (float.Parse(balanceEth) < float.Parse(balanceEth1))
        {
            Debug.Log("transfer successfully");
            if (indexValue == 1)
            {
                //Buy Gold
                if (hasAddGold == false)
                {
                    BlockchainEffect.Instance.goldBought += goldValue;
                    goldBoughtText.text = "Gold Bought: " + BlockchainEffect.Instance.goldBought.ToString();
                    hasAddGold = true;
                    UpdateStatus("Bought 10,000 Gold");
                }

            }
            else if (indexValue == 2)
            {
                //Buy Gem
                if (hasAddGem == false)
                {
                    BlockchainEffect.Instance.gemBought += gemValue;
                    gemBoughtText.text = "Gem Bought: " + BlockchainEffect.Instance.gemBought.ToString();
                    hasAddGem = true;
                    UpdateStatus("Bought 100 Gem");
                }
            }
        }
    }

    private void BoughtSuccessFully(int indexValue)
    {
        if (indexValue == 1)
        {
            BlockchainEffect.Instance.goldBought += goldValue;
            goldBoughtText.text = "Gold Bought: " + BlockchainEffect.Instance.goldBought.ToString();
            hasAddGold = true;
            UpdateStatus("Bought 10,000 Gold");
        }
        else if (indexValue == 2)
        {
            BlockchainEffect.Instance.gemBought += gemValue;
            gemBoughtText.text = "Gem Bought: " + BlockchainEffect.Instance.gemBought.ToString();
            hasAddGem = true;
            UpdateStatus("Bought 100 Gem");
        }        
    }

    public async void BuyGoldAndMeat(int indexValue)
    {
        HideAllButtons();
        UpdateStatus("Buying...");
        BigInteger weiAmount = ConvertToWei(itemPrice);
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log("balanceEth1: " + balanceEth);
        if (float.Parse(balanceEth) < itemPrice)
        {
            UpdateStatus("Not Enough Token...");
            ShowAllButtons();
            return;
        }

        // Bắt đầu Coroutine
        StartCoroutine(WaitAndExecute(indexValue));       
        try
        {
            await wallet.Transfer(ChainId, receiverAddress, weiAmount);
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred during the transfer: {ex.Message}");
        }
    }    

    // Coroutine chờ 3 giây trước khi thực hiện
    IEnumerator WaitAndExecute(int indexValue)
    {
        Debug.Log("Coroutine started, waiting for 3 seconds...");
        yield return new WaitForSeconds(3f); // Chờ 3 giây
        Debug.Log("3 seconds have passed!");
        BoughtSuccessFully(indexValue);
        ShowAllButtons();
    }

    public void ChangeToScenePlay()
    {
        SceneManager.LoadScene("Loading");
    }
}
