using MaterialData.exceptions;
using MaterialData.interfaces;
using MaterialData.models;
using MaterialData.repository;
using System.Collections.Generic;
using System.Linq;

namespace MaterialLogic
{
    public class DisplayLogic : BaseLogic<display>, IMaterialLogic
    {
        public DisplayLogic(BaseRepository<display> Repo) : base(Repo)
        {
        }

        public override void IsValid(display item)
        {
            List<string> errList = new List<string>();
            if (string.IsNullOrEmpty(item.make))
                errList.Add("𝗠𝗮𝗿𝗸𝗲");

            if (string.IsNullOrEmpty(item.model))
                errList.Add("𝗠𝗼𝗱𝗲𝗹𝗹");

            var existingItem = Repo.Entities.Set<display>().FirstOrDefault(x => x.serial_number == item.serial_number);
            if (existingItem != null && item.id != existingItem.id && item.serial_number != "")
                throw new DuplicateEntryException($"Seriennummer \"{item.serial_number}\" in Datenbank bereits vorhanden! \n({existingItem.make} {existingItem.model})");

            if (errList.Count > 0)
            {
                string err = BuildErrorMessage(errList);
                throw new InvalidInputException(err);
            }

            if (!item.serial_number.Equals(""))
                item.quantity = 1;

            if (item.quantity < 1 || item.quantity == null)
                throw new InvalidInputException("Anzahl darf nicht kleiner als 1 sein!");
        }

        public override display SetLocation(display item)
        {
            if (item.location_id == null)
                item.location_id = Repo.defaultLocation;

            item = AddIfExisting(item);

            if (item != null)
                item = RebookItem(item);

            return item;
        }

        private display AddIfExisting(display item)
        {
            display existingDisplay = Repo.Entities.Set<display>().FirstOrDefault(x => x.make == item.make && x.model == item.model && x.location_id == item.location_id);
            display sameDisplayInDb = Repo.Entities.Set<display>().FirstOrDefault(x => x.id == item.id);


            if (existingDisplay != null)
            {
                if (sameDisplayInDb != null && existingDisplay.id == sameDisplayInDb.id)
                    return item;

                if (sameDisplayInDb != null && sameDisplayInDb.quantity < item.quantity && item.id != existingDisplay.id)
                {
                    var word1 = "";
                    var word2 = "";
                    if (existingDisplay.quantity == 1) { word1 += "kann"; word2 += "ist"; } else { word1 += "können"; word2 += "sind"; }
                    throw new InvalidInputException($"Es können nicht {item.quantity}x \"{item.make} {item.model}\" umgebucht werden, da nur {sameDisplayInDb.quantity} lagernd {word2}!");
                }

                existingDisplay.quantity += item.quantity;

                if (sameDisplayInDb != null)
                {
                    sameDisplayInDb.quantity -= item.quantity;
                    if (sameDisplayInDb.quantity <= 0)
                        Repo.Entities.display.Remove(sameDisplayInDb);
                    else
                        Repo.Entities.display.Update(sameDisplayInDb);
                }

                Repo.Entities.display.Update(existingDisplay);
                Repo.Entities.SaveChanges();

                item = null;
            }
            return item;
        }

        private display RebookItem(display item)
        {
            var existingDisplay = Repo.Entities.Set<display>().FirstOrDefault(x => x.id == item.id);
            if (existingDisplay != null)
            {
                if (item.id == existingDisplay.id && item.id == 0)
                    return item;

                if (existingDisplay.quantity < item.quantity && item.id != existingDisplay.id)
                {
                    var word1 = "";
                    var word2 = "";
                    if (existingDisplay.quantity == 1) { word1 += "kann"; word2 += "ist"; } else { word1 += "können"; word2 += "sind"; }
                    throw new InvalidInputException($"Es können nicht {item.quantity}x \"{item.make} {item.model}\" umgebucht werden, da nur {existingDisplay.quantity} lagernd {word2}!");

                }

                existingDisplay.quantity -= item.quantity;

                if (existingDisplay.quantity <= 0)
                    Repo.Entities.display.Remove(existingDisplay);
                else
                    Repo.Entities.display.Update(existingDisplay);

                item.id = 0;
                Repo.Entities.display.Add(item);
                Repo.Entities.SaveChanges();
                item = null;
            }
            return item;
        }
    }
}