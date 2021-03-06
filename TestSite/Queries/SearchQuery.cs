﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RealEstateCompanyWebSite.Queries;

namespace RealEstateCompanyWebSite
{
    /// <summary>
    /// Query class to query advanced features. Able to search for keywords, property features and location
    /// </summary>
    public class SearchQuery : IQuery
    {
        MySqlConnection connection;

        /// <summary>
        /// list of keywords to search for
        /// </summary>
        public string[] Keywords { get; set; }
        
        /// <summary>
        /// Minimum listing price
        /// </summary>
        public float PriceMin { get; set; }

        /// <summary>
        /// Maximum listing price
        /// </summary>
        public float PriceMax { get; set; }
        
        /// <summary>
        /// Minimum number of bedrooms
        /// </summary>
        public int BedroomsMin { get; set; }
        
        /// <summary>
        /// Maximum number of bedrooms
        /// </summary>
        public int BedroomsMax { get; set; }
        
        /// <summary>
        /// Minimum number of bathrooms
        /// </summary>
        public int BathroomsMin { get; set; }
        
        /// <summary>
        /// Maximum number of bathrooms
        /// </summary>
        public int BathroomsMax { get; set; }
        
        /// <summary>
        /// Minimum number of car parking spaces
        /// </summary>
        public int GaragesMin { get; set; }
        
        /// <summary>
        /// Maximum number of car parking spaces
        /// </summary>
        public int GaragesMax { get; set; }
        
        /// <summary>
        /// Minimum area of plot
        /// </summary>
        public int PlotSizeMin { get; set; }
        
        /// <summary>
        /// Maximum area of plot
        /// </summary>
        public int PlotSizeMax { get; set; }
        
        /// <summary>
        /// Minimum area of house
        /// </summary>
        public int HouseSizeMin { get; set; }
        
        /// <summary>
        /// Maximum area of house
        /// </summary>
        public int HouseSizeMax { get; set; }
        
        /// <summary>
        /// Province of property
        /// </summary>
        public int ProvinceId { get; set; }
        
        /// <summary>
        /// City of property
        /// </summary>
        public int City_Id { get; set; }
        
        /// <summary>
        /// Area (location) of property
        /// </summary>
        public int Area_Id { get; set; }

        /// <summary>
        /// Indicates if a property should have a pool
        /// null for any
        /// </summary>
        public bool? HasPool { get; set; }


        int limit = 10;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string for database</param>
        public SearchQuery(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();

         
            ProvinceId = -1;
            City_Id = -1;
            Area_Id = -1;

            const int MIN = 0;
            const int MAX = 50000000;
            BedroomsMin = MIN;
            BedroomsMax = MAX;
            BathroomsMin = MIN;
            BathroomsMax = MAX;
            GaragesMin = MIN;
            GaragesMax = MAX;
            PlotSizeMin = MIN;
            PlotSizeMax = MAX;
            HouseSizeMin = MIN;
            HouseSizeMax = MAX;
            PriceMin = MIN;
            PriceMax = MAX;
            HasPool = null;
            this.Keywords = null;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            connection.Close();
        }

        /// <summary>
        /// Split keywords into array of keywords
        /// </summary>
        /// <param name="keywords"></param>
        public void SetKeywords(string keywords)
        {
            SetKeywords(keywords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Assign list of keywords
        /// </summary>
        /// <param name="keywords"></param>
        public void SetKeywords(string[] keywords)
        {
            this.Keywords = keywords;
        }

     
        private MySqlDataReader BuildQuery()
        {


            string qry = @"select
Listing.List_ID,
List_Price,
Property.Client_ID,
Property.Complex_ID,
Property.Property_Unit_No,
Address_Streetno,
Address_Streetname,
Property_Bedroom_Count,
Property_Bathroom_Count,
Property_Garage_Count,
Property_hasPool,
Property_Plot_Size,
Property_House_Size,
Property_Value,
Area_Name,
City_Name,
Province_Name,
Image_URL
from Property, Listing, Address, Area, City, Province, Image
where
(Property.Property_Bedroom_Count between @bed_min and @bed_max) and
(Property.Property_Bathroom_Count between @bath_min and @bath_max) and
(Property.Property_Garage_Count between @gar_min and @gar_max) and
(Property.Property_Plot_Size between @plot_min and @plot_max) and
(Property.Property_House_Size between @house_min and @house_max) and
(Listing.List_Price between @list_min and @list_max) and
Property.Property_ID = Listing.Property_ID and
Property.Address_ID = Address.Address_ID and
Address.Area_ID = Area.Area_ID and
City.City_ID = Area.Area_City_ID and
Province.Province_ID = City.City_Province_ID and
Image.Property_ID = Property.Property_ID";






            // Optional parameters
            if(HasPool != null)
            {
                qry += "@ and (Property.Property_hasPool  = @hasPool)";
            }
            if(ProvinceId >= 0)
            {
                qry += @" and (Province.Province_ID = @province_id)";
            }
            if(Area_Id >= 0)
            {
                qry += @" and Area.Area_ID = @area_id";
            }
            if(City_Id >= 0)
            {
                qry += @" and City.City_ID = @city_id";
            }
            int k = 0;
            if(Keywords != null && Keywords.Length > 0)
            {
                foreach (var keyword in Keywords)
                {
                    // string keywordNo = string.Format("@k{0}",k++);
                    qry += string.Format(" and (Address_Streetname like '%{0}%' or Province_Name like '%{0}%' or City_Name like '%{0}%' or Property_Description like '%{0}%')", keyword);
                    if (k < Keywords.Length - 1)
                        qry += " or ";
                    //command.Parameters.AddWithValue(keywordNo, keyword);
                    // TO DO SANITIZE!

                }
            }


            // Finalize command
            qry += @"
group by
Image.Property_ID 
limit @limit;";

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = qry;

            // Optional parameters
            if (HasPool != null)
            {
                int hasPool = HasPool == true ? 1 : 0;
                command.Parameters.AddWithValue("@hasPool", hasPool);
            }
            if (ProvinceId >= 0)
            {
                command.Parameters.AddWithValue("@province_id", ProvinceId);
            }
            if (Area_Id >= 0)
            {
                command.Parameters.AddWithValue("@area_id", Area_Id);
            }
            if (City_Id >= 0)
            {
                command.Parameters.AddWithValue("@city_id", City_Id);
            }


            // Assign values
            command.Parameters.AddWithValue("@bed_min", BedroomsMin);
            command.Parameters.AddWithValue("@bed_max", BedroomsMax);
            command.Parameters.AddWithValue("@bath_min", BathroomsMin);
            command.Parameters.AddWithValue("@bath_max", BathroomsMax);
            command.Parameters.AddWithValue("@gar_min", GaragesMin);
            command.Parameters.AddWithValue("@gar_max", GaragesMax);
            command.Parameters.AddWithValue("@plot_min", PlotSizeMin);
            command.Parameters.AddWithValue("@plot_max", PlotSizeMax);
            command.Parameters.AddWithValue("@house_min", HouseSizeMin);
            command.Parameters.AddWithValue("@house_max", HouseSizeMax);
            command.Parameters.AddWithValue("@list_min", PriceMin);
            command.Parameters.AddWithValue("@list_max", PriceMax);
            command.Parameters.AddWithValue("@limit", 20);

            try
            {
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Return the results
        /// </summary>
        /// <returns></returns>
        public MySqlDataReader GenerateResults()
        {
            return BuildQuery();
        }
    }
}