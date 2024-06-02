using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class ShopController
    {

        public static Home Window { get; set; }

        // Lijst met bestelde producten in de 'winkelmand'
        public List<OrderedProduct> ShoppingCart = new List<OrderedProduct>();

        public void Start()
        {
            // DONE Maak een lijst van producten
            List<KioskProduct> products = new List<KioskProduct>
            {
                new KioskProduct { Name = "Foto 10x15", Price = 2.55M },
                new KioskProduct { Name = "Mok", Price = 5.99M },
                new KioskProduct { Name = "Muismat 45x30", Price = 14.99M },
                new KioskProduct { Name = "Bidon 500cl", Price = 8.99M },
                new KioskProduct { Name = "Teddybear", Price = 8.55M }
            };

            // DONE Genereer de prijslijst
            string priceList = "Prijzen:\n\n";
            foreach (var product in products)
            {
                priceList += $"{product.Name}: €{product.Price:F2}\n";
            }

            // Stel de prijslijst in
            ShopManager.SetShopPriceList(priceList);

            // Vul de productlijst met producten
            ShopManager.Products.Clear(); // Zorg ervoor dat de lijst leeg is voordat je deze opnieuw vult
            ShopManager.Products.AddRange(products); // Vul de productlijst opnieuw met de nieuwe lijst van producten

            // We stellen een placeholder in voor de bon
            ShopManager.AddShopReceipt("Producten:\n\n");
            ShopManager.AddShopReceipt("Eindbedrag: €0");

            // Update dropdown met producten
            ShopManager.UpdateDropDownProducts();
        }

        // Wordt uitgevoerd wanneer er op de Toevoegen knop is geklikt
        public void AddButtonClick()
        {
            // We halen de gekozen foto-Id op uit de form.
            int? pictureId = ShopManager.GetFotoId();

            // We kijken of de foto-Id een geldige waarde heeft. Zoniet geven we een waarschuwing en verlaten we de functie
            // (We zouden ook moeten kijken of er effectief een foto met dit Id bestaat)
            if (!pictureId.HasValue)
            {
                MessageBox.Show("Voer een geldig foto-ID in!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // We halen het gekozen product op uit de form.
            KioskProduct product = ShopManager.GetSelectedProduct();

            // We kijken of het gekozen product een geldige waarde heeft. Zoniet geven we een waarschuwing en verlaten we de functie
            if (product == null)
            {
                MessageBox.Show("Kies een product!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // We halen het gekozen aantal op uit de form.
            int? amount = ShopManager.GetAmount();

            // We kijken of het gekozen aantal een geldige waarde heeft. Zoniet geven we een waarschuwing en verlaten we de functie
            if (!amount.HasValue)
            {
                MessageBox.Show("Voer een geldig aantal in!", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // We maken een object aan van het type OrderedProduct en geven hier alle nodige informatie aan mee van het gekozen product
            OrderedProduct newOrderedProduct = new OrderedProduct()
            {
                PictureId = pictureId.Value,
                ProductName = product.Name,
                Amount = amount.Value,
                TotalPrice = product.Price * amount.Value
            };                  

            // We voegen het gekozen product toe aan de winkelmand
            ShoppingCart.Add(newOrderedProduct);

            // We zetten een hoofding in het label voor het receipt
            ShopManager.SetShopReceipt("Producten: \n\n");

            // We initialiseren een variabele om de totale prijs in op te slaan
            Decimal totalPrice = 0;

            // We loopen door alle producten in de winkelmand
            foreach (OrderedProduct orderedProduct in ShoppingCart) 
            {
                // We voegen een regel toe op de bon met het gekozen product. We laten het aantal, de product naam en de totale prijs zien
                ShopManager.AddShopReceipt($"{orderedProduct.Amount} x {orderedProduct.ProductName} = €{orderedProduct.TotalPrice}\n");
                // In dezelfde loop berekenen we ook de totale prijs door de totale prijs van het huidige product in de loop toe te voegen aan de totalPrice variabele
                totalPrice += orderedProduct.TotalPrice;
            }

            // Spaties
            ShopManager.AddShopReceipt("\n\n");
            // We printen de totale prijs op de bon
            ShopManager.AddShopReceipt($"Eindbedrag: €{totalPrice}");

            // We maken de form velden terug leeg
            ShopManager.ClearInputFields();
        }

        // Wordt uitgevoerd wanneer er op de Resetten knop is geklikt
        public void ResetButtonClick()
        {
            // Wanneer er op de Resetten knop wordt gedrukt moet de staat van de winkel 'gereset' worden

            // We maken de winkelmand leeg
            ShoppingCart.Clear();
            // We maken de form velden terug leeg
            ShopManager.ClearInputFields();
            // We maken de bon terug leeg
            ShopManager.ClearShopReceipt();
        }

        // Wordt uitgevoerd wanneer er op de Save knop is geklikt
        public void SaveButtonClick()
        {
            // Wanneer er op de Save knop wordt gedrukt moet de huidige bon worden opgeslagen in een tekst bestand

            // De bestandsnaam van de bon
            string fileName = "bon_" + DateTime.Now.ToString("dd_MM_yy_HH_mm_ss") + ".txt";

            // We schrijven de huidige bon weg naar het bestand
            File.WriteAllText(@"../../../bonnen/" + fileName, ShopManager.GetShopReceipt());

            // We resetten de staat van de winkel om de verkoop af te sluiten
            ResetButtonClick();
        }

    }
}