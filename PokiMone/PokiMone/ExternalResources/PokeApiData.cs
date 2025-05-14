using PokeApiNet;

namespace PokiMone.ExternalResources
{
    public class PokeApiData
    {
        private static readonly PokeApiClient pokeClient = new();

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

        public async Task<List<Move>> GetAllMovesAsync()
        {
            List<string> moveNameList = [];

            await foreach (var move in pokeClient.GetAllNamedResourcesAsync<Move>())
            {
                moveNameList.Add(move.Name);
            }

            List<Move> moveList = [];
            foreach (var moveName in moveNameList)
            {
                moveList.Add(await pokeClient.GetResourceAsync<Move>(moveName));
            }

            return moveList;
        }

        public async Task<List<Move>> GetPokemonMoveTypes(Pokemon pokemon)
        {
            var throttler = new SemaphoreSlim(30); // Limit to 30 concurrent requests

            var moveTasks = pokemon.Moves.Select(async m =>
            {
                await throttler.WaitAsync();
                try
                {
                    return await pokeClient.GetResourceAsync<Move>(m.Move);
                }
                finally
                {
                    throttler.Release();
                }
            });

            return [.. await Task.WhenAll(moveTasks)];
        }
    }
}
