namespace RainWorldCE.Config.CustomChaos
{
    internal class CCGoto : CCEntry
    {
        int line;

        public CCGoto(int line)
        {
            this.line = line;
        }

        public override int doAction()
        {
            //-2 because 0 index and because step will tick up by 1 at the end of current cycle
            CustomChaos.step = line - 2;
            return 0;
        }

        public override string ToString()
        {
            return $"Go to line {line}";
        }
    }
}
