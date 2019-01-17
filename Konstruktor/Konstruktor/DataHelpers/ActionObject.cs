namespace Konstruktor.DataHelpers
{
    public enum ActionType { Add, Remove }

    public class ActionObject
    {
        public Cuboid Cuboid { get; set; }
        public ActionType Action { get; set; }

        public ActionObject(Cuboid cuboid, ActionType action)
        {
            Cuboid = cuboid;
            Action = action;
        }
    }
}
