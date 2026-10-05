using System;
using System.Collections.Generic;

namespace ServerCore.CustomCommands
{
    public class CheckVehicleContent : ConsoleCmdAbstract
    {
        public override string getDescription()
        {
            return "check the content of a vehicle.";
        }

        public override string getHelp()
        {
            return "cvc <vehicleID>";
        }

        public override string[] getCommands()
        {
            return new[] { "pc-cvc", "cvc", "checkvehiclecontent" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (!int.TryParse(_params[0], out int bikeID))
                {
                    SdtdConsole.Instance.Output("ERR: Invalid vehicleID format!");
                    return;
                }

                List<Entity> entityList = GameManager.Instance.World.Entities.list;
                bool vehicleFound = false;



                for (int i = 0; i < entityList.Count; i++)
                {
                    Entity entity = entityList[i];

                    if (entity is EntityVBlimp)
                    {
                        EntityVBlimp entityBlimp = (EntityVBlimp)entity;
                        if (entityBlimp.entityId == bikeID)
                        {
                            //get the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityBlimp.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Jetpack owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityBlimp.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityBlimp.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }


                            vehicleFound = true;
                            break;
                        }
                    }

                    if (entity is EntityVHelicopter)
                    {
                        EntityVHelicopter entityHelicopter = (EntityVHelicopter)entity;
                        if (entityHelicopter.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityHelicopter.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Helicopter owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityHelicopter.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityHelicopter.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }


                            vehicleFound = true;
                            break;
                        }
                    }

                    if (entity is EntityVJeep)
                    {
                        EntityVJeep entityJeep = (EntityVJeep)entity;
                        if (entityJeep.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityJeep.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Jeep owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityJeep.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityJeep.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }


                            vehicleFound = true;
                            break;
                        }
                    }

                    if (entity is EntityMotorcycle)
                    {
                        EntityMotorcycle entityMotorcycle = (EntityMotorcycle)entity;
                        if (entityMotorcycle.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityMotorcycle.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Motorcycle owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityMotorcycle.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityMotorcycle.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }


                            vehicleFound = true;
                            break;
                        }

                    }

                    if (entity is EntityVGyroCopter)
                    {
                        EntityVGyroCopter entityGyrocopter = (EntityVGyroCopter)entity;
                        if (entityGyrocopter.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityGyrocopter.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Gyrocopter owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityGyrocopter.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityGyrocopter.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }

                            vehicleFound = true;
                            break;
                        }

                    }

                    if (entity is EntityBicycle)
                    {
                        EntityBicycle entityBicycle = (EntityBicycle)entity;
                        if (entityBicycle.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityBicycle.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Bicycle owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityBicycle.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityBicycle.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }

                            vehicleFound = true;
                            break;
                        }

                    }

                    if (entity is EntityMinibike)
                    {
                        EntityMinibike entityMinibike = (EntityMinibike)entity;

                        if (entityMinibike.entityId == bikeID)
                        {
                            //find the current owner
                            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(entityMinibike.GetOwner());

                            if (playerDataFromEntityID != null)
                            {
                                SdtdConsole.Instance.Output("Minibike owner: " + playerDataFromEntityID.PlayerName.DisplayName);
                            }

                            ItemStack[] bikestack = entityMinibike.bag.ItemGrid.CloneItems();

                            SdtdConsole.Instance.Output("Items in storage:");
                            if (entityMinibike.bag.IsEmpty())
                            {
                                SdtdConsole.Instance.Output("Storage is empty!");
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ItemName : count : quality");
                                foreach (ItemStack itemstack in bikestack)
                                {
                                    if (!itemstack.IsEmpty())
                                    {
                                        if (itemstack.itemValue.HasQuality)
                                        {
                                            SdtdConsole.Instance.Output(string.Format("{0} : {1} : {2}", itemstack.itemValue.ItemClass.Name, itemstack.count, itemstack.itemValue.Quality.ToString()));

                                        }
                                        else SdtdConsole.Instance.Output(string.Format("{0} : {1}", itemstack.itemValue.ItemClass.Name, itemstack.count));
                                    }
                                }
                            }

                            vehicleFound = true;
                            break;
                        }
                    }
                }
                if (!vehicleFound) SdtdConsole.Instance.Output("ERR: Vehicle not found!");
            }
            catch (Exception e)
            {
                Log.Out("Error in CheckVehicleContent.Run: " + e);
            }

        }
    }
}
