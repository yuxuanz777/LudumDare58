using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text totalValueText;
    [SerializeField] Transform marketContent;   // 指向 MarketScrollView/Viewport/Content
    [SerializeField] Transform libraryContent;  // 指向 LibraryScrollView/Viewport/Content

    [Header("Prefabs")]
    [SerializeField] MarketCardView marketCardPrefab;
    [SerializeField] LibraryRowView libraryRowPrefab;

    List<GameItem> market = new List<GameItem>();
    List<GameItem> library = new List<GameItem>();
    int money = 1000;

    void Start()
    {
        UpdateTopBar();
        GenerateMarket();
        RefreshMarketUI();
        RefreshLibraryUI();
    }

    // 随机生成可购买的搞笑游戏 //
    void GenerateMarket(int count = 3)
    {
        market.Clear();
        for (int i = 0; i < count; i++)
        {
            var item = new GameItem(
                System.Guid.NewGuid().ToString(),
                GenFunnyName(),
                Random.Range(50, 300),
                Random.Range(50, 300),
                RandomGenre()
            );
            market.Add(item);
        }
    }
    // 刷新商店 //
    void RefreshMarketUI()
    {
        marketContent.DestroyAllChildren();
        foreach (var item in market)
        {
            var card = Instantiate(marketCardPrefab, marketContent);
            card.Bind(item, OnBuyClicked, money >= item.price);
        }
    }

    // 刷新收藏库 //
    void RefreshLibraryUI()
    {
        libraryContent.DestroyAllChildren();
        foreach (var item in library)
        {
            var row = Instantiate(libraryRowPrefab, libraryContent);
            row.Bind(item, OnSellClicked);  // ← 把卖出逻辑传下去
        }
        UpdateTopBar();
    }

    void UpdateTopBar()
    {
        moneyText.text = $"Money: ${money}";
        totalValueText.text = $"Total Value: ${CalcTotalValue()}";
    }

    int CalcTotalValue()
    {
        int sum = 0; 
        foreach (var i in library) 
            sum += i.value; return sum;
    }

    // 购买 //
    public void OnBuyClicked(GameItem item)
    {
        if (money < item.price) return;  // 买不起TT
        money -= item.price;
        library.Add(item);
        market.Remove(item);
        RefreshMarketUI();
        RefreshLibraryUI();
    }

    // 卖出 //
    public void OnSellClicked(GameItem item)
    {
        //money += item.value;
        money += Mathf.RoundToInt(item.value * 0.8f); // 卖出加成 80%
        library.Remove(item);
        RefreshLibraryUI();
        RefreshMarketUI();
    }

    // 在 EndTurnButton 的 OnClick 里绑定这个方法
    public void OnEndTurn()
    {
        // 市场刷新
        GenerateMarket();
        RefreshMarketUI();

        // 收藏价值波动
        for (int i = 0; i < library.Count; i++)
        {
            int delta = Random.Range(-20, 40);
            library[i].value = Mathf.Max(0, library[i].value + delta);
        }
        RefreshLibraryUI();
    }

    // 随机生成搞笑的游戏名字 //
    static readonly string[] A = { "Ultra", "Super", "Mega", "Dark", "Cyber", "Rogue", "Neo", "Pocket" };
    static readonly string[] B = { "Dungeon", "Kart", "Chef", "Farmer", "Survivor", "Hacker", "Cat", "Wizard" };
    static readonly string[] C = { " Simulator", " Tycoon", " Royale", " 2077", " Remake", " Origins", " DX" };
    string GenFunnyName() => $"{A[Random.Range(0, A.Length)]} {B[Random.Range(0, B.Length)]}{C[Random.Range(0, C.Length)]}";
    static readonly string[] Genres = { "RPG", "Shooter", "Casual", "Strategy", "Indie" };
    string RandomGenre() => Genres[Random.Range(0, Genres.Length)];
}

// —— 安全清空子物体？ —— //
static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(t.GetChild(i).gameObject);
        }
    }
}
