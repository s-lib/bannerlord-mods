namespace SoundTheAlarm {
    public class STAConfiguration {

        public bool EnableVillagePopup { get; set; } = true;
        public bool EnableCastlePopup { get; set; } = true;
        public bool EnableTownPopup { get; set; } = true;
        public bool EnableWarPopup { get; set; } = true;
        public bool EnablePeacePopup { get; set; } = true;
        public bool PauseGameOnPopup { get; set; } = true;
        public float TimeToRemoveVillageFromList { get; set; } = 10.0f;
        public bool EnableDebugMessages { get; set; } = false;

    }
}
