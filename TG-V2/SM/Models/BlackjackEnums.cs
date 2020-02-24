namespace SM
{
    public enum GameState : int
    {
        InitialState = 0,
        PlayerTurn,
        Stand,
        Hit,
        DoubleDown,
        PlayerBlackjack,
        Split,
        Bust,
        FinalState
    }
    public enum BlackjackMove : int
    {
        Hit = 0,
        Stand,
        Split,
        DoubleDown
    }
}