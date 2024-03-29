﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AdsProject.BussinessEntities;
using Microsoft.EntityFrameworkCore;

namespace AdsProject.DataAccessLogic
{
    public class AdDAL
    { 
        public static async Task<int> CreateAsync(Ad ad)
        {
            int result = 0;
            using(var bdContexto = new ContextoDB())
            {
                bdContexto.Add(ad);
                await bdContexto.SaveChangesAsync();
            }
            return result;
        }
        public static async Task<int> UpdateAsycn(Ad ad)
        {
            int result = 0;
            using (var bdContexto = new ContextoDB())
            {
                var adDb = await bdContexto.Ad.FirstOrDefaultAsync(a => a.Id == ad.Id);
                if (adDb != null)
                {
                    adDb.IdCategory = ad.IdCategory;
                    adDb.Title = ad.Title;
                    adDb.Description = ad.Description;
                    adDb.Price = ad.Price;
                    adDb.RegistrationDate = ad.RegistrationDate;
                    adDb.State = ad.State;

                    bdContexto.Update(adDb);
                    result = await bdContexto.SaveChangesAsync();

                }
                return result;
            }
            
        }
        public static async Task<int> DeleteAsync(Ad ad)
        {
            int result = 0;
            using(var bdContexto = new ContextoDB())
            {
                var AdDB = await bdContexto.Ad.FirstOrDefaultAsync(a => a.Id == ad.Id);
                if(AdDB != null)
                {
                    bdContexto.Ad.Remove(AdDB);
                    result = await bdContexto.SaveChangesAsync();
                }
            }
            return result;
        }
        public static async Task<Ad> GetByIdAsync(Ad ad)
        {
            var adDB = new Ad();
            using(var bdContexto = new ContextoDB())
            {
                adDB = await bdContexto.Ad.FirstOrDefaultAsync(a => a.Id == ad.Id);
            }
            return adDB;
        }
        public static async Task<List<Ad>> GetAllAsync()
        {
            var ads = new List<Ad>();
            using(var bdContexto = new ContextoDB())
            {
                ads = await bdContexto.Ad.ToListAsync();
            }
            return ads;
        }
        internal static IQueryable<Ad> QuerySelect(IQueryable<Ad> query, Ad ad)
        {
            if (ad.Id > 0)
                query = query.Where(a => a.Id == ad.Id);
            if(ad.IdCategory > 0)
                query = query.Where(a => a.IdCategory == ad.IdCategory);
            if(!string.IsNullOrWhiteSpace(ad.Title))
                query = query.Where(a => a.Title == ad.Title);
            if (!string.IsNullOrWhiteSpace(ad.Description))
                query = query.Where(a => a.Description == ad.Description);
            if(ad.Price > 0)
                query = query.Where(a => a.Price == ad.Price);
            if(ad.RegistrationDate.Year > 1900) // para fechas 
            {
                DateTime inicialDate = new DateTime(ad.RegistrationDate.Year, ad.RegistrationDate.Month, ad.RegistrationDate.Day, 0, 0, 0);
               
                DateTime finalDate = inicialDate.AddDays(1).AddMilliseconds(-1);

                query = query.Where(a => a.RegistrationDate > inicialDate && a.RegistrationDate < finalDate);

            }
            if (!string.IsNullOrWhiteSpace(ad.State))
                query = query.Where(a => a.State.Contains(ad.State));

            query = query.OrderByDescending(a =>  a.Id).AsQueryable();
            if(ad.Top_Aux > 0)
                query= query.Take(ad.Top_Aux).AsQueryable();
            return query;

        }
        public static async Task<List<Ad>> SearchAsync(Ad ad )
        {
            var ads = new List<Ad>();
            using(var bdContexto = new ContextoDB())
            {
                var select = bdContexto.Ad.AsQueryable();
                select = QuerySelect(select, ad);
                ads = await select.ToListAsync();
            }
            return ads;
        }
        public static async Task<List<Ad>> SearchIncludeCategoryAsync(Ad ad)
        {
            var ads = new List<Ad>();
            using(var bdContexto = new ContextoDB())
            {
                var select = bdContexto.Ad.AsQueryable();
                select = QuerySelect(select, ad).Include(a => a.Category).AsQueryable();
                ads = await select.ToListAsync();
            }
            return ads;
        }
    }
}
