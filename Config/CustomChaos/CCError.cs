namespace RainWorldCE.Config.CustomChaos
{
    internal class CCError : CCEntry
    {
        public override int doAction()
        {
            return 1;
        }

        public override string ToString()
        {
            return "ERROR";
        }
    }
}
