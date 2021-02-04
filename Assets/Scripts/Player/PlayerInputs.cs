namespace Player
{
    public static class PlayerInputs
    {
        private static PlayerInActions _controls;

        public static PlayerInActions PlayerControls
        {
            get
            {
                if (_controls != null) return _controls;
                return _controls = new PlayerInActions();
            }
        }
    }
}