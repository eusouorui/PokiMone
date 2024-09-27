using PokeApiNet;

namespace PokiMone.ExternalResources
{
    public class PokeApiData
    {
        private static readonly PokeApiClient pokeClient = new();
        private readonly IConfiguration _config;

        public PokeApiData(IConfiguration config)
        {
            _config = config;
        }

        public void MongoDbAccess()
        {
            var firstPart = _config.GetValue<string>("MongoDb:ConnectionStringP1");
            var secondPart = _config.GetValue<string>("MongoDb:ConnectionStringP2");
            var password = _config.GetValue<string>("password");
        }

        public async Task<Pokemon> GetPokemonDataAsync(dynamic pokemonName)
        {
            Pokemon pokemon = await pokeClient.GetResourceAsync<Pokemon>(pokemonName);

            return pokemon;
        }

        public async Task<List<Pokemon>> GetPokemonRange(int limit = 151)
        {
            List<string> pokemonNameList = [];

            await foreach (var pokemon in pokeClient.GetAllNamedResourcesAsync<Pokemon>())
            {
                pokemonNameList.Add(pokemon.Name);

                if (pokemonNameList.Count == limit) break;
            }

            List<Pokemon> pokemonList = [];

            foreach (var pokemonName in pokemonNameList)
            {
                pokemonList.Add(await GetPokemonDataAsync(pokemonName));
            }

            return pokemonList;
        }
    }
}
