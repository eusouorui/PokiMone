namespace PokiMone.Entities
{
    public class Pokemon : BaseEntity
    {
        public int InternationalDexNumber { get; set; }
        public PokemonStats Stats { get; set; }
        public List<Type>? Types { get; set; }

    }
}
