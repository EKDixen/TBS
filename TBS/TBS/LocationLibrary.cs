public static class LocationLibrary
{


    public static Location starterTown = new Location(true, "StarterTown", new System.Numerics.Vector2(0, 0), new List<SubLocation>
    {
        /*new SubLocation("StoreName", SubLocationType.Shop)
        {
            shopItems = new List<Item>
            {
                new Item("lwk test item", "+10 DMG", 16,2 , ItemType.equipment)
                {
                    stats= {["DMG"] = 10 }
                    //details="shi this lwk just a test"
                },
                new Item("lwk test item 2", "+15 speed", 16,2 , ItemType.equipment)
                {
                    stats= {["speed"] = 15 }
                    //details=" yeah lwk just a test mate"
                }

            }
        }*/
    });
    

    public static Location forest = new Location(false, "Forest", new System.Numerics.Vector2(1, 0), new List<SubLocation>
    {
        new SubLocation("name", SubLocationType.Shop)
        {
            shopItems = new List<Item>
            {
                new Item("lwk test item", "+10 DMG", 16,2 , ItemType.equipment)
                {
                    stats= {["DMG"] = 10 },
                    details="shi this lwk just a test"
                },
                new Item("lwk test item 2", "+15 speed", 16,2 , ItemType.equipment)
                {
                    stats= {["speed"] = 15 },
                    details=" yeah lwk just a test mate"
                }

            }
        }
    });

    public static Location mountain = new Location(false, "Mountain", new System.Numerics.Vector2(-1, 0), new List<SubLocation>
    {

    });

    public static Location lake = new Location(false, "Lake", new System.Numerics.Vector2(0, -1), new List<SubLocation>
    {

    });




    public static List<Location> locations = new List<Location>
    {
        starterTown, forest, mountain, lake
    };

}

