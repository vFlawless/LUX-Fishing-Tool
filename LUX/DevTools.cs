﻿using Google.Cloud.Firestore;

namespace LUX
{
    [FirestoreData]
    public class Previous_Payload
    {
        [FirestoreProperty]
        public int amount { get; set; }

        [FirestoreProperty]
        public float avrgMultiplier { get; set; }

        [FirestoreProperty]
        public int amountGreen { get; set; }

        [FirestoreProperty]
        public int amountBlue { get; set; }

        [FirestoreProperty]
        public int amountPurple { get; set; }

        [FirestoreProperty]
        public int amountOrange { get; set; }

        [FirestoreProperty]
        public int amountRed { get; set; }

        [FirestoreProperty]
        public float highestMultiplier { get; set; }

        [FirestoreProperty]
        public string planet { get; set; }

        [FirestoreProperty]
        public string URLSubstring { get; set; }
    }

    public class DevTools
    {

        public static async void ConvertData2(FirestoreDb db)
        {
            // Function that will convert old Data (with single float instead of top 3 for highest multiplier)
            DocumentReference doc = db.Collection("URLS").Document("Domain"); // Collection with the names of the other collections (bases for leaderboards)
            DocumentSnapshot snapshot = await doc.GetSnapshotAsync();
            
            Payload2 pl = snapshot.ConvertTo<Payload2>();
            for (int i = 0; i < pl.Domains.Count; i++)     // For every collection (domain of URL)
            {
                Query allURL = db.Collection(pl.Domains[i]);
                QuerySnapshot allURLs = await allURL.GetSnapshotAsync();

                for (int j = 0; j < allURLs.Count; j++) // For every URL in collection
                {
                    string ID = allURLs[j].Id;
                    if(ID != "02VOylXbGlVvkN6yXEEW")
                    {
                        Previous_Payload URL = allURLs[j].ConvertTo<Previous_Payload>();

                        Payload payload3 = new Payload
                        {
                            amount = URL.amount,
                            amountGreen = URL.amountGreen,
                            amountBlue = URL.amountBlue,
                            amountPurple = URL.amountPurple,
                            amountOrange = URL.amountOrange,
                            amountRed = URL.amountRed,
                            avrgMultiplier = URL.avrgMultiplier,
                            highestMultiplier = new List<float>() { URL.highestMultiplier },
                            planet = URL.planet,
                            URLSubstring = URL.URLSubstring
                        };

                        DocumentReference docRef = db.Collection(pl.Domains[i]).Document(ID);
                        await docRef.SetAsync(payload3);
                    }
                }
            }
        }

        public static async void TransferData(FirestoreDb db)
        {
            DocumentReference docRef = db.Collection("").Document("");
            DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();
            string URLSUB = "";
            string Domain = "";

            Payload URL = documentSnapshot.ConvertTo<Payload>();
            URL.URLSubstring = URLSUB;
            Query capitalQuery = db.Collection(Domain).WhereEqualTo("URLSubstring", URLSUB);
            QuerySnapshot capitalQuerySnapshot = await capitalQuery.GetSnapshotAsync();

            if (capitalQuerySnapshot.Count == 0)
            {
                await db.Collection(Domain).Document("").SetAsync(URL);
            }
            else
            {
                // If URL already exists (previously fished on)
                string ID = capitalQuerySnapshot[0].Id; // Get ID of Document (for changing later on)

                float average = URL.avrgMultiplier * URL.amount;     //Calculate the average Multiplier

                Payload pl = capitalQuerySnapshot[0].ConvertTo<Payload>();
                if (URL.planet == "Null" || URL.amount <= 4) // Gotta check this
                {
                    URL.planet = pl.planet;
                }
                URL.amount += pl.amount;
                URL.amountGreen += pl.amountGreen;
                URL.amountBlue += pl.amountBlue;
                URL.amountPurple += pl.amountPurple;
                URL.amountOrange += pl.amountOrange;
                URL.amountRed += pl.amountRed;
                URL.highestMultiplier = GetTop3Highest(pl.highestMultiplier, URL.highestMultiplier);
                average += pl.avrgMultiplier * pl.amount;
                average /= URL.amount;

                URL.avrgMultiplier = average;


                // Update Values for URL:
                DocumentReference document = db.Collection(Domain).Document(ID);
                await document.SetAsync(URL);
            }

        }

        private static List<float> GetTop3Highest(List<float> list1, List<float> list2)
        {
            // Combine the two lists into one
            var combinedList = list1.Concat(list2);
            // Order the combined list in descending order by value
            var top3 = combinedList.OrderByDescending(f => f).Take(3);
            // return the top 3 highest floats
            return top3.ToList();
        }

        public static async void ConvertData(FirestoreDb db)    // Converts from old data to new version
        {
            /*
            string planet;
            string URLSub;
            float avrg;

            DocumentReference docRef = db.Collection("").Document("");
            DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();

            Previous_Payload URL = documentSnapshot.ConvertTo<Previous_Payload>();
            planet = URL.Planet;
            URLSub = URL.URLSubstring;

            List<float> Multipliers = new();
            float highest = 0;

            for (int i = 0; i < URL.GreenMultipliers.Count; i++)
            {
                if (URL.GreenMultipliers[i] >= 1000)
                {
                    Multipliers.Add(URL.GreenMultipliers[i] / 1000);
                    highest = URL.GreenMultipliers[i] / 1000 > highest ? URL.GreenMultipliers[i] / 1000 : highest;
                }
                else
                {
                    Multipliers.Add(URL.GreenMultipliers[i]);
                    highest = URL.GreenMultipliers[i] > highest ? URL.GreenMultipliers[i] : highest;
                }
            }
            for (int i = 0; i < URL.BlueMultipliers.Count; i++)
            {
                if (URL.BlueMultipliers[i] >= 1000)
                {
                    Multipliers.Add(URL.BlueMultipliers[i] / 1000);
                    highest = URL.BlueMultipliers[i] / 1000 > highest ? URL.BlueMultipliers[i] / 1000 : highest;
                }
                else
                {
                    Multipliers.Add(URL.BlueMultipliers[i]);
                    highest = URL.BlueMultipliers[i] > highest ? URL.BlueMultipliers[i] : highest;
                }
            }
            for (int i = 0; i < URL.PurpleMultipliers.Count; i++)
            {
                if (URL.PurpleMultipliers[i] >= 1000)
                {
                    Multipliers.Add(URL.PurpleMultipliers[i] / 1000);
                    highest = URL.PurpleMultipliers[i] / 1000 > highest ? URL.PurpleMultipliers[i] / 1000 : highest;
                }
                else
                {
                    Multipliers.Add(URL.PurpleMultipliers[i]);
                    highest = URL.PurpleMultipliers[i] > highest ? URL.PurpleMultipliers[i] : highest;
                }
            }
            for (int i = 0; i < URL.OrangeMultipliers.Count; i++)
            {
                if (URL.OrangeMultipliers[i] >= 1000)
                {
                    Multipliers.Add(URL.OrangeMultipliers[i] / 1000);
                    highest = URL.OrangeMultipliers[i] / 1000 > highest ? URL.OrangeMultipliers[i] / 1000 : highest;
                }
                else
                {
                    Multipliers.Add(URL.OrangeMultipliers[i]);
                    highest = URL.OrangeMultipliers[i] > highest ? URL.OrangeMultipliers[i] : highest;
                }
            }
            for (int i = 0; i < URL.RedMultipliers.Count; i++)
            {
                if (URL.RedMultipliers[i] >= 1000)
                {
                    Multipliers.Add(URL.RedMultipliers[i] / 1000);
                    highest = URL.RedMultipliers[i] / 1000 > highest ? URL.RedMultipliers[i] / 1000 : highest;
                }
                else
                {
                    Multipliers.Add(URL.RedMultipliers[i]);
                    highest = URL.RedMultipliers[i] > highest ? URL.RedMultipliers[i] : highest;
                }
            }
            avrg = Multipliers.Average();
            Payload payload3 = new Payload
            {
                amount = Multipliers.Count,
                amountGreen = URL.GreenMultipliers.Count,
                amountBlue = URL.BlueMultipliers.Count,
                amountPurple = URL.PurpleMultipliers.Count,
                amountOrange = URL.OrangeMultipliers.Count,
                amountRed = URL.RedMultipliers.Count,
                avrgMultiplier = avrg,
                highestMultiplier = highest,
                planet = planet,
                URLSubstring = URLSub
            };
            await docRef.SetAsync(payload3);
            */
        }


        public static async void ResetData(FirestoreDb db)  // Resets all the data in DB
        {
            // Function that will fix the uploaded data
            DocumentReference doc = db.Collection("URLS").Document("Domain"); // Collection with the names of the other collections (bases for leaderboards)
            DocumentSnapshot snapshot = await doc.GetSnapshotAsync();
            string planet;
            string URLSub;

            Payload2 pl = snapshot.ConvertTo<Payload2>();
            for (int i = 0; i < pl.Domains.Count; i++)     // For every collection (domain of URL)
            {
                Query allURL = db.Collection(pl.Domains[i]);
                QuerySnapshot allURLs = await allURL.GetSnapshotAsync();

                for (int j = 0; j < allURLs.Count; j++) // For every URL in collection
                {
                    string ID = allURLs[j].Id;
                    Payload URL = allURLs[j].ConvertTo<Payload>();
                    planet = URL.planet;
                    URLSub = URL.URLSubstring;

                    Payload payload3 = new Payload
                    {
                        amount = 0,
                        amountGreen = 0,
                        amountBlue = 0,
                        amountPurple = 0,
                        amountOrange = 0,
                        amountRed = 0,
                        avrgMultiplier = 0,
                        highestMultiplier = new List<float>(),
                        planet = planet,
                        URLSubstring = URLSub
                    };
                    DocumentReference docRef = db.Collection(pl.Domains[i]).Document(ID);
                    await docRef.SetAsync(payload3);
                }
            }
        }
    }
}
