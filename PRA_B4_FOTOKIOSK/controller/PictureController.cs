using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class PictureController
    {
        // De window die we laten zien op het scherm
        public static Home Window { get; set; }


        // De lijst met fotos die we laten zien
        public List<KioskPhoto> PicturesToDisplay = new List<KioskPhoto>();
        
        
        // Start methode die wordt aangeroepen wanneer de foto pagina opent.
        public void Start()
        {
            // We berekenen de onder en boven grens van 30 minuten en 2 minuten geleden.
            // We declareren deze variabelen buiten de foreach loop omdat de waarde ervan constant blijft
            DateTime ThirtyMinutesAgo = DateTime.Now.AddMinutes(-30);
            DateTime TwoMinutesAgo = DateTime.Now.AddMinutes(-2);

            // Initializeer de lijst met fotos
            // WAARSCHUWING. ZONDER FILTER LAADT DIT ALLES!
            // foreach is een for-loop die door een array loopt
            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                // Index van vandaag
                int indexToday = (int)DateTime.Now.DayOfWeek;

                // Index van de huidige directory in de foreach
                int indexDirectory = int.Parse(Path.GetFileName(dir).Substring(0, 1));

                // Als de index van vandaag niet gelijk is aan de index van de directory slaan we deze over. Zo tonen we enkel foto's van vandaag.
                if (!indexToday.Equals(indexDirectory))
                    continue;

                /**
                 * dir string is de map waar de fotos in staan. Bijvoorbeeld:
                 * \fotos\0_Zondag
                 */

                // We sorteren de lijst van fotos op naam reversed zodat de meest recent fotos vanboven komen te staan.
                List<String> files = Directory.GetFiles(dir)
                             .OrderBy(file => Path.GetFileName(file))
                             .Reverse()
                             .ToList();

                foreach (string file in files)
                {
                    // We parsen de tijd in de foto naam en zetten deze in een DateTime.
                    DateTime pictureTime; 
                    String pictureTimeString = Path.GetFileName(file).Substring(0, 8);
                    DateTime.TryParseExact(pictureTimeString, "HH_mm_ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out pictureTime);

                    //Als de foto ouder is dan 30 minuten of nieuwer dan 2 minuten laten we deze niet zien.
                    if (pictureTime < ThirtyMinutesAgo || pictureTime > TwoMinutesAgo)
                        continue;

                    /**
                     * file string is de file van de foto. Bijvoorbeeld:
                     * \fotos\0_Zondag\10_05_30_id8824.jpg
                     */
                    int idIndex = file.IndexOf("id");
                    // We halen de id van de foto uit de foto naam.
                    int id = int.Parse(file.Substring(idIndex + 2, 4));
                    PicturesToDisplay.Add(new KioskPhoto() { Id = id, Source = file });
                }
            }

            // Update de fotos
            PictureManager.UpdatePictures(PicturesToDisplay);
        }

        // Wordt uitgevoerd wanneer er op de Refresh knop is geklikt
        public void RefreshButtonClick()
        {
            // We maken eerst het scherm leeg en daarna roepen we de Start function weer op om de meest recent foto's te laten zien.
            PicturesToDisplay.Clear();
            Start();
        }

    }
}
