namespace Lab08.Interfaces
{
    public interface ISense
    {
        bool Detect(Game game);
        void Notify(Game game);
    }
}