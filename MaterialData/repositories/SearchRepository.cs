﻿using MaterialData.models;
using MaterialData.models.material;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MaterialData.repositories
{
    public class SearchRepository
    {
        private DcvEntities Entities;

        public SearchRepository(DcvEntities entities)
        {
            Entities = entities;
        }

        public Dictionary<string, List<Material>> GetResult(search search)
        {

            Dictionary<string, List<Material>> searchList = new Dictionary<string, List<Material>>();


            if (search.category.Count <= 0)
            {
                string searchString = search.searchKeyWord;

                string[] test = { "" };
                if (searchString.Contains(" "))
                {
                    test = searchString.Split(' ');
                }
                else
                {
                    test[0] = searchString;
                }

                List<Material> notebooks = new List<Material>();
                List<Material> displays = new List<Material>();
                List<Material> furnitures = new List<Material>();
                List<Material> books = new List<Material>();
                List<Material> equipments = new List<Material>();

                notebooks.AddRange(Entities.notebook.Where(x => x.make.Contains(searchString) || x.model.Contains(searchString) || x.serial_number.Contains(searchString) || x.person.name1.Contains(searchString) || x.person.name2.Contains(searchString) || x.classroom.room.Contains(searchString) || x.classroom.addressloc.address.place.Contains(searchString) || x.classroom.addressloc.address.street.Contains(searchString) || x.classroom.addressloc.address.zip.ToString().Contains(searchString) || x.classroom.addressloc.address.country.Contains(searchString)).Include(x => x.person).Include(x => x.classroom).ThenInclude(x => x.addressloc).ThenInclude(x => x.address).AsNoTracking().ToList());
                displays.AddRange(Entities.display.Where(x => x.make.Contains(searchString) || x.model.Contains(searchString) || x.serial_number.Contains(searchString) || x.classroom.room.Contains(searchString) || x.classroom.addressloc.address.place.Contains(searchString) || x.classroom.addressloc.address.street.Contains(searchString) || x.classroom.addressloc.address.zip.ToString().Contains(searchString) || x.classroom.addressloc.address.country.Contains(searchString)).Include(x => x.classroom).ThenInclude(x => x.addressloc).ThenInclude(x => x.address).AsNoTracking().ToList());
                furnitures.AddRange(Entities.furniture.Where(x => x.type.Contains(searchString) || x.classroom.room.Contains(searchString) || x.classroom.addressloc.address.place.Contains(searchString) || x.classroom.addressloc.address.street.Contains(searchString) || x.classroom.addressloc.address.zip.ToString().Contains(searchString) || x.classroom.addressloc.address.country.Contains(searchString)).Include(x => x.classroom).ThenInclude(x => x.addressloc).ThenInclude(x => x.address).AsNoTracking().ToList());
                books.AddRange(Entities.book.Where(x => x.title.Contains(searchString) || x.isbn.Contains(searchString) || x.classroom.room.Contains(searchString) || x.classroom.addressloc.address.place.Contains(searchString) || x.classroom.addressloc.address.street.Contains(searchString) || x.classroom.addressloc.address.zip.ToString().Contains(searchString) || x.classroom.addressloc.address.country.Contains(searchString)).Include(x => x.person).Include(x => x.classroom).ThenInclude(x => x.addressloc).ThenInclude(x => x.address).AsNoTracking().ToList());
                equipments.AddRange(Entities.equipment.Where(x => x.type.Contains(searchString) || x.make.Contains(searchString) || x.model.Contains(searchString) || x.classroom.room.Contains(searchString) || x.classroom.addressloc.address.place.Contains(searchString) || x.classroom.addressloc.address.street.Contains(searchString) || x.classroom.addressloc.address.zip.ToString().Contains(searchString) || x.classroom.addressloc.address.country.Contains(searchString)).Include(x => x.person).Include(x => x.classroom).ThenInclude(x => x.addressloc).ThenInclude(x => x.address).AsNoTracking().ToList());
                if (test != null)
                {
                    foreach (var str in test)
                    {
                        var notebook = Entities.notebook.Where(x => x.person.name1 == str || x.person.name2 == str).ToList();
                        var book = Entities.book.Where(x => x.title.Contains(str) || x.person.name1 == str || x.person.name2 == str).ToList();
                        var equipment = Entities.equipment.Where(x => x.person.name1 == str || x.person.name2 == str).ToList();

                        foreach (var n in notebook)
                        {
                            if (notebooks.FirstOrDefault(x => x.id == n.id) == null)
                                notebooks.Add(n);
                        }
                        foreach (var b in book)
                        {
                            if (books.FirstOrDefault(x => x.id == b.id) == null)
                                books.Add(b);
                        }
                        foreach (var e in equipment)
                        {
                            if (equipments.FirstOrDefault(x => x.id == e.id) == null)
                                equipments.Add(e);
                        }
                    }
                }
                searchList.Add("notebook", notebooks);
                searchList.Add("display", displays);
                searchList.Add("furniture", furnitures);
                searchList.Add("book", books);
                searchList.Add("equipment", equipments);
            }

            else
            {
                foreach (var item in search.category)
                {
                    if (item.Equals("notebook"))
                    {
                        List<Material> notebooks = new List<Material>();
                        notebooks.AddRange(Entities.notebook.ToList());
                        searchList.Add("Notebooks", notebooks);
                    }

                    if (item.Equals("display"))
                    {
                        List<Material> displays = new List<Material>();
                        displays.AddRange(Entities.display.ToList());
                        searchList.Add("Bildschirme", displays);
                    }

                    if (item.Equals("book"))
                    {
                        List<Material> books = new List<Material>();
                        books.AddRange(Entities.book.ToList());
                        searchList.Add("Bücher", books);
                    }

                    if (item.Equals("equipment"))
                    {
                        List<Material> equipments = new List<Material>();
                        equipments.AddRange(Entities.equipment.ToList());
                        searchList.Add("Zubehör", equipments);
                    }

                    if (item.Equals("furniture"))
                    {
                        List<Material> furnitures = new List<Material>();
                        furnitures.AddRange(Entities.furniture.ToList());
                        searchList.Add("Mobiliar", furnitures);
                    }
                }
            }
            return searchList;
        }
    }
}