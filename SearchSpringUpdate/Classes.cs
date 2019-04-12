using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SearchSpringUpdate
{
    //SearchSpring XML Classes
    /*https://xmltocsharp.azurewebsites.net/ */
    [XmlRoot(ElementName = "Custom")]
    public class Custom
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Image")]
    public class Image
    {
        [XmlElement(ElementName = "Thumbnail")]
        public string Thumbnail { get; set; }
        [XmlElement(ElementName = "Large")]
        public string Large { get; set; }
        [XmlElement(ElementName = "PopUp")]
        public string PopUp { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "inactive")]
        public string Inactive { get; set; }
        [XmlElement(ElementName = "Link")]
        public string Link { get; set; }
    }

    [XmlRoot(ElementName = "Images")]
    public class Images
    {
        [XmlElement(ElementName = "Image")]
        public List<Image> Image { get; set; }
        [XmlAttribute(AttributeName = "replace_existing")]
        public string Replace_existing { get; set; }
    }

    [XmlRoot(ElementName = "Price")]
    public class Price
    {
        [XmlElement(ElementName = "StandardPrice")]
        public string StandardPrice { get; set; }
        [XmlElement(ElementName = "SpecialPrice")]
        public string SpecialPrice { get; set; }
        [XmlAttribute(AttributeName = "price_category")]
        public string Price_category { get; set; }
    }

    [XmlRoot(ElementName = "Retail")]
    public class Retail
    {
        [XmlElement(ElementName = "Price")]
        public Price Price { get; set; }
        [XmlAttribute(AttributeName = "active")]
        public string Active { get; set; }
    }

    [XmlRoot(ElementName = "InventoryControl")]
    public class InventoryControl
    {
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "Inventory")]
        public string Inventory { get; set; }
        [XmlElement(ElementName = "OutOfStockPoint")]
        public string OutOfStockPoint { get; set; }
        [XmlElement(ElementName = "OnOrder")]
        public string OnOrder { get; set; }
        [XmlAttribute(AttributeName = "ignore_backorder")]
        public string Ignore_backorder { get; set; }
        [XmlAttribute(AttributeName = "inventory_control_exempt")]
        public string Inventory_control_exempt { get; set; }
    }

    [XmlRoot(ElementName = "SellInSets")]
    public class SellInSets
    {
        [XmlElement(ElementName = "QuantityInSet")]
        public string QuantityInSet { get; set; }
        [XmlElement(ElementName = "NumIterationsDisplayed")]
        public string NumIterationsDisplayed { get; set; }
    }

    [XmlRoot(ElementName = "QuantityRestrictions")]
    public class QuantityRestrictions
    {
        [XmlElement(ElementName = "MinimumQuantity")]
        public string MinimumQuantity { get; set; }
        [XmlElement(ElementName = "MaximumQuantity")]
        public string MaximumQuantity { get; set; }
        [XmlElement(ElementName = "SellInSets")]
        public SellInSets SellInSets { get; set; }
    }

    [XmlRoot(ElementName = "Weight")]
    public class Weight
    {
        [XmlElement(ElementName = "ShipWeight")]
        public string ShipWeight { get; set; }
        [XmlElement(ElementName = "DisplayWeight")]
        public string DisplayWeight { get; set; }
        [XmlElement(ElementName = "DisplayUnit")]
        public string DisplayUnit { get; set; }
    }

    [XmlRoot(ElementName = "Shipping")]
    public class Shipping
    {
        [XmlElement(ElementName = "ShipPreference")]
        public string ShipPreference { get; set; }
        [XmlElement(ElementName = "FixedRate")]
        public string FixedRate { get; set; }
        [XmlElement(ElementName = "PromoFixedRate")]
        public string PromoFixedRate { get; set; }
    }

    [XmlRoot(ElementName = "Meta")]
    public class Meta
    {
        [XmlElement(ElementName = "Keywords")]
        public string Keywords { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "Categories")]
    public class Categories
    {
        [XmlElement(ElementName = "ID")]
        public List<string> ID { get; set; }
        [XmlAttribute(AttributeName = "replace_existing")]
        public string Replace_existing { get; set; }
    }

    [XmlRoot(ElementName = "categories")]
    public class categories
    {
        [XmlElement(ElementName = "category")]
        public List<Category> category { get; set; }
        [XmlAttribute(AttributeName = "invisible")]
        public string invisible { get; set; }
        [XmlAttribute(AttributeName = "featured")]
        public string featured { get; set; }
    }

    [XmlRoot(ElementName = "SubProduct")]
    public class SubProduct
    {
        [XmlElement(ElementName = "SKU")]
        public string SKU { get; set; }
        [XmlElement(ElementName = "ProdID")]
        public string ProdID { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Image")]
        public Image Image { get; set; }
        [XmlElement(ElementName = "Retail")]
        public Retail Retail { get; set; }
        [XmlElement(ElementName = "InventoryControl")]
        public InventoryControl InventoryControl { get; set; }
        [XmlElement(ElementName = "QuantityRestrictions")]
        public QuantityRestrictions QuantityRestrictions { get; set; }
        [XmlElement(ElementName = "Weight")]
        public Weight Weight { get; set; }
        [XmlElement(ElementName = "Shipping")]
        public Shipping Shipping { get; set; }
        [XmlElement(ElementName = "TaxCode")]
        public string TaxCode { get; set; }
        [XmlElement(ElementName = "DateCreated")]
        public string DateCreated { get; set; }
        [XmlAttribute(AttributeName = "inactive")]
        public string Inactive { get; set; }
        [XmlAttribute(AttributeName = "out_of_season")]
        public string Out_of_season { get; set; }
        [XmlAttribute(AttributeName = "tax_exempt")]
        public string Tax_exempt { get; set; }
        [XmlAttribute(AttributeName = "google_checkout_exempt")]
        public string Google_checkout_exempt { get; set; }
        [XmlAttribute(AttributeName = "is_donation")]
        public string Is_donation { get; set; }
        [XmlAttribute(AttributeName = "allow_fractional_qty")]
        public string Allow_fractional_qty { get; set; }
    }

    [XmlRoot(ElementName = "SubProducts")]
    public class SubProducts
    {
        [XmlElement(ElementName = "SubProduct")]
        public List<SubProduct> SubProduct { get; set; }
        [XmlAttribute(AttributeName = "active")]
        public string Active { get; set; }
    }

    [XmlRoot(ElementName = "Title")]
    public class Title
    {
        [XmlAttribute(AttributeName = "column")]
        public string Column { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Titles")]
    public class Titles
    {
        [XmlElement(ElementName = "Title")]
        public List<Title> Title { get; set; }
    }

    [XmlRoot(ElementName = "Value")]
    public class Value
    {
        [XmlAttribute(AttributeName = "column")]
        public string Column { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Combination")]
    public class Combination
    {
        [XmlElement(ElementName = "Value")]
        public List<Value> Value { get; set; }
    }

    [XmlRoot(ElementName = "Codes")]
    public class Codes
    {
        [XmlElement(ElementName = "Value")]
        public List<Value> Value { get; set; }
    }

    [XmlRoot(ElementName = "GiftCertificate")]
    public class GiftCertificate
    {
        [XmlElement(ElementName = "DaysAvailable")]
        public string DaysAvailable { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "active")]
        public string Active { get; set; }
    }

    [XmlRoot(ElementName = "Attribute")]
    public class Attribute
    {
        [XmlElement(ElementName = "SKU")]
        public string SKU { get; set; }
        [XmlElement(ElementName = "AltID")]
        public string AltID { get; set; }
        [XmlElement(ElementName = "ShipWeight")]
        public string ShipWeight { get; set; }
        [XmlElement(ElementName = "Combination")]
        public Combination Combination { get; set; }
        [XmlElement(ElementName = "Codes")]
        public Codes Codes { get; set; }
        [XmlElement(ElementName = "Retail")]
        public Retail Retail { get; set; }
        [XmlElement(ElementName = "Special")]
        public string Special { get; set; }
        [XmlElement(ElementName = "InventoryControl")]
        public InventoryControl InventoryControl { get; set; }
        [XmlElement(ElementName = "TaxCode")]
        public string TaxCode { get; set; }
        [XmlElement(ElementName = "GiftCertificate")]
        public GiftCertificate GiftCertificate { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "is_donation")]
        public string Is_donation { get; set; }
    }

    [XmlRoot(ElementName = "Attributes")]
    public class Attributes
    {
        [XmlElement(ElementName = "Titles")]
        public Titles Titles { get; set; }
        [XmlElement(ElementName = "Attribute")]
        public List<Attribute> Attribute { get; set; }
        [XmlAttribute(AttributeName = "active")]
        public string Active { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {
        public int ourID { get; set; }
        [XmlElement(ElementName = "SKU")]
        public string SKU { get; set; }
        [XmlElement(ElementName = "ProdID")]
        public string ProdID { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "URLName")]
        public string URLName { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Keywords")]
        public string Keywords { get; set; }
        [XmlElement(ElementName = "Custom")]
        public List<Custom> Custom { get; set; }
        [XmlElement(ElementName = "Brand")]
        public string Brand { get; set; }
        [XmlElement(ElementName = "Manufacturer")]
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "Rating")]
        public string Rating { get; set; }
        [XmlElement(ElementName = "Images")]
        public Images Images { get; set; }
        [XmlElement(ElementName = "Retail")]
        public Retail Retail { get; set; }
        [XmlElement(ElementName = "InventoryControl")]
        public InventoryControl InventoryControl { get; set; }
        [XmlElement(ElementName = "QuantityRestrictions")]
        public QuantityRestrictions QuantityRestrictions { get; set; }
        [XmlElement(ElementName = "Weight")]
        public Weight Weight { get; set; }
        [XmlElement(ElementName = "Shipping")]
        public Shipping Shipping { get; set; }
        [XmlElement(ElementName = "TaxCode")]
        public string TaxCode { get; set; }
        [XmlElement(ElementName = "Meta")]
        public Meta Meta { get; set; }
        [XmlElement(ElementName = "Categories")]
        public Categories Categories { get; set; }
        [XmlElement(ElementName = "DefaultCategory")]
        public string DefaultCategory { get; set; }
        [XmlElement(ElementName = "CategoryFilters")]
        public CategoryFilters CategoryFilters { get; set; }
        [XmlElement(ElementName = "SubProducts")]
        public SubProducts SubProducts { get; set; }
        [XmlElement(ElementName = "Attributes")]
        public Attributes Attributes { get; set; }
        [XmlElement(ElementName = "DateCreated")]
        public string DateCreated { get; set; }
        [XmlAttribute(AttributeName = "inactive")]
        public string Inactive { get; set; }
        [XmlAttribute(AttributeName = "out_of_season")]
        public string Out_of_season { get; set; }
        [XmlAttribute(AttributeName = "tax_exempt")]
        public string Tax_exempt { get; set; }
        [XmlAttribute(AttributeName = "text_field")]
        public string Text_field { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "featured")]
        public string Featured { get; set; }
        [XmlAttribute(AttributeName = "new")]
        public string New { get; set; }
        [XmlAttribute(AttributeName = "comparable")]
        public string Comparable { get; set; }
        [XmlAttribute(AttributeName = "google_checkout_exempt")]
        public string Google_checkout_exempt { get; set; }
        [XmlAttribute(AttributeName = "content_only")]
        public string Content_only { get; set; }
        [XmlAttribute(AttributeName = "is_donation")]
        public string Is_donation { get; set; }
        [XmlAttribute(AttributeName = "kit")]
        public string Kit { get; set; }
        [XmlAttribute(AttributeName = "allow_fractional_qty")]
        public string Allow_fractional_qty { get; set; }
    }

    [XmlRoot(ElementName = "products")]
    public class Products
    {
        [XmlElement(ElementName = "product")]
        public List<Product> ProductList { get; set; }
    }

    [XmlRoot(ElementName = "CV3Data")]
    public class CV3Data
    {
        [XmlElement(ElementName = "products")]
        public Products Products { get; set; }
        [XmlElement(ElementName = "copyright")]
        public string Copyright { get; set; }
        [XmlElement(ElementName = "categories")]
        public categories categories { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "CategoryFilter")]
    public class CategoryFilter
    {
        [XmlElement(ElementName = "Filter")]
        public string Filter { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "SortValue")]
        public string SortValue { get; set; }
    }

    [XmlRoot(ElementName = "CategoryFilters")]
    public class CategoryFilters
    {
        [XmlElement(ElementName = "CategoryFilter")]
        public List<CategoryFilter> CategoryFilter { get; set; }
    }


    ////////////////////////////////////////////////////////////Category////////////////////////////////////////////////////////////
    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "URLName")]
        public string URLName { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "MetaTitle")]
        public string MetaTitle { get; set; }
        [XmlElement(ElementName = "MetaDescription")]
        public string MetaDescription { get; set; }
        [XmlElement(ElementName = "MetaKeywords")]
        public string MetaKeywords { get; set; }
        [XmlElement(ElementName = "Template")]
        public string Template { get; set; }
        [XmlElement(ElementName = "NumProductsPerPage")]
        public string NumProductsPerPage { get; set; }
        /*[XmlElement(ElementName = "Products")]
        public Products Products { get; set; }*/
        [XmlElement(ElementName = "FeaturedProducts")]
        public string FeaturedProducts { get; set; }
        [XmlElement(ElementName = "CategoryPath")]
        public string CategoryPath { get; set; }
        [XmlElement(ElementName = "Custom")]
        public List<Custom> Custom { get; set; }
        [XmlAttribute(AttributeName = "invisible")]
        public string Invisible { get; set; }
        [XmlAttribute(AttributeName = "featured")]
        public string Featured { get; set; }
    }
}
