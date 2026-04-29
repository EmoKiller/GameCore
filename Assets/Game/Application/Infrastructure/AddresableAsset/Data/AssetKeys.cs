namespace Game.Application.Modules.Assets
{
    public static class AssetKeys
    {
        // Nhóm các tài nguyên dùng chung toàn game
        public static class Labels
        {
            public const string StaticGlobal = "static_global";
            public const string Startup = "startup_assets";
        }
        public static class Global
        {
            public const string Label = "static_global";
            public const string MainConfig = "config_gameSettings";
            public const string CommonAtlas = "atlas_commonUI";
        }

        // Nhóm các tài nguyên theo từng Module/Scene
        public static class UI
        {
            public const string LoadingScreen = "ui_loadingscreen";
            public const string MainMenu = "ui_mainmenu";
            public const string HUD = "ui_gamehud";
        }

        public static class Audio
        {
            public const string BGM_Lobby = "bgm_lobby_loop";
            public const string SFX_Click = "sfx_ui_click";
        }
    }
}
