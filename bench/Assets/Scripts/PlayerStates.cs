public static class PlayerStats
{
    private static int coinsNum = 0;
    private static int[] foodAvailable = new int[] { 0, 0 };
   
    public static int CoinsNum
    {
        get
        {
            return coinsNum;
        }
        set
        {
            coinsNum = value;
        }
    }

    public static int[] FoodAvailable 
    {
        get
        {
            return foodAvailable;
        }
        set
        {
            foodAvailable = value;
        }
    }
}

