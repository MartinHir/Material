﻿using MaterialData.exceptions;
using MaterialData.interfaces;
using MaterialData.models;
using MaterialData.repository;
using System.Collections.Generic;
using System.Linq;

namespace MaterialLogic
{
    public class FurnitureLogic : BaseLogic<furniture>, IMaterialLogic
    {
        public FurnitureLogic(BaseRepository<furniture> Repo) : base(Repo)
        {
        }

        public override void IsValid(furniture item)
        {
            List<string> errList = new List<string>();
            if (string.IsNullOrEmpty(item.type))
                errList.Add("𝗔𝗿𝘁");

            if (item.quantity == null)
                errList.Add("𝗔𝗻𝘇𝗮𝗵𝗹");

            if (errList.Count > 0)
            {
                string err = BuildErrorMessage(errList);
                throw new InvalidInputException(err);
            }

            if (item.quantity < 1)
                throw new InvalidInputException("Anzahl darf nicht kleiner als 1 sein!");
        }

        public override furniture SetLocation(furniture item)
        {
            if (item.location_id == null)
                item.location_id = Repo.defaultLocation;
            
            item = AddIfExisting(item);
            
            if (item != null)
                item = RebookItem(item);

            return item;
        }

        private furniture AddIfExisting(furniture item)
        {
            furniture existingFurniture = Repo.Entities.Set<furniture>().FirstOrDefault(x => x.type == item.type && x.location_id == item.location_id);
            furniture sameFurnitureInDb = Repo.Entities.Set<furniture>().FirstOrDefault(x => x.id == item.id);
            if (existingFurniture != null)
            {
                //Repo.GetRelation();
                existingFurniture.quantity += item.quantity;

                if (sameFurnitureInDb != null)
                {
                    sameFurnitureInDb.quantity -= item.quantity;
                    if (sameFurnitureInDb.quantity <= 0)
                        Repo.Entities.furniture.Remove(sameFurnitureInDb);
                    else
                        Repo.Entities.furniture.Update(sameFurnitureInDb);
                }

                Repo.Entities.furniture.Update(existingFurniture);
                Repo.Entities.SaveChanges();

                item = null;
            }
            return item;
        }

        private furniture RebookItem(furniture item)
        {
            var existingFurniture = Repo.Entities.Set<furniture>().FirstOrDefault(x => x.id == item.id);
            if (existingFurniture != null)
            {
                existingFurniture.quantity -= item.quantity;

                if (existingFurniture.quantity <= 0)
                    Repo.Entities.furniture.Remove(existingFurniture);
                else
                    Repo.Entities.furniture.Update(existingFurniture);

                item.id = 0;
                Repo.Entities.furniture.Add(item);
                Repo.Entities.SaveChanges();
                item = null;
            }
            return item;
        }
    }
}