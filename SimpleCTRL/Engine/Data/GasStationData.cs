namespace SimpleCTRL.Engine.Data
{
    [XmlRoot(ElementName = "Pump")]
    public class GasPump
    {
        #region Properties

        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "Position")]
        public Vector3 Position { get; set; }

        [XmlElement(ElementName = "Rotation")]
        public float Rotation { get; set; }

        #endregion
    }

    [XmlRoot(ElementName = "Pumps")]
    public class Pumps
    {
        #region Properties

        [XmlElement(ElementName = "Pump")]
        public List<GasPump> PumpList { get; set; }

        #endregion
    }

    [XmlRoot(ElementName = "GasStation")]
    public class GasStation
    {
        #region Properties

        [XmlElement(ElementName = "Position")]
        public Vector3 Position { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Pumps")]
        public Pumps Pumps { get; set; }

        #endregion

        #region Methods

        internal static List<GasStation> LoadFromFile(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GasStations));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                var gasStations = (GasStations)serializer.Deserialize(fileStream);
                return gasStations.GasStationList;
            }
        }

        internal void CreateBlip(bool hasElectricPumps)
        {
            Blip blip = new Blip(Position);

            if (hasElectricPumps)
            {
                blip.Sprite = (BlipSprite)620;
                blip.Color = Color.Yellow;
                blip.Name = "Electric Charging Station";
            }
            else
            {
                blip.Sprite = (BlipSprite)361;
                blip.Color = Color.White;
                blip.Name = "Gas Station";
            }

            blip.Scale = 1f;
            NativeFunction.CallByHash<int>(0xBE8BE4FE60E27B72, blip, true); // SET_BLIP_AS_SHORT_RANGE
            GasStationMarkers.Add(blip);
        }

        public static GasStation GetClosestInRange(Vector3 pos, float rangeSquared)
        {
            return AvailableGasStations
                .Where(gs => Vector3.DistanceSquared(gs.Position, pos) < rangeSquared)
                .OrderBy(gs => Vector3.DistanceSquared(gs.Position, pos))
                .FirstOrDefault();
        }

        #endregion
    }

    [XmlRoot(ElementName = "GasStations")]
    public class GasStations
    {
        #region Properties

        [XmlElement(ElementName = "GasStation")]
        public List<GasStation> GasStationList { get; set; }

        #endregion

        #region Methods

        internal static void LoadGasStationsFromFile()
        {
            string configFilePath = Path.Combine(Settings.PluginPath, @"data\GasStations.xml");

            if (File.Exists(configFilePath))
            {
                Logging.Info($"Loading gas stations from file: {configFilePath}", "GasStationData");
                try
                {
                    AvailableGasStations = GasStation.LoadFromFile(configFilePath);
                    Logging.Info($"Loaded {AvailableGasStations.Count} gas stations.", "GasStationData");
                }
                catch (Exception ex)
                {
                    Logging.Error($"Error loading gas stations from file: {configFilePath}", "GasStationData", ex);
                }
            }
            else
            {
                Logging.Warning($"Configuration file not found: {configFilePath}", "GasStationData");
            }
        }

        internal static void LoadExtendedStationsFromFile()
        {
            string configFilePath = Path.Combine(Settings.PluginPath, @"data\ExtendedStations.xml");

            if (File.Exists(configFilePath))
            {
                Logging.Info($"Loading extended stations from file: {configFilePath}", "GasStationData");
                try
                {
                    var extendedStations = GasStation.LoadFromFile(configFilePath);
                    AvailableGasStations.AddRange(extendedStations);
                    foreach (GasStation extendedStation in extendedStations)
                    {
                        AvailableExtendedStations.AddRange(extendedStation.Pumps.PumpList);
                    }
                    Logging.Info($"Loaded {extendedStations.Count} extended stations.", "GasStationData");
                }
                catch (Exception ex)
                {
                    Logging.Error($"Error loading extended stations from file: {configFilePath}", "GasStationData", ex);
                }
            }
            else
            {
                Logging.Warning($"Configuration file not found: {configFilePath}", "GasStationData");
            }
        }

        internal static void LoadAllStations()
        {
            LoadGasStationsFromFile();
            LoadExtendedStationsFromFile();
        }

        #endregion
    }
}