using PokeApiNet;
using PokiMone.ExternalResources;

namespace PokiMone.Services
{
    public class CacheService
    {
        private readonly PokeApiData ApiLayer;
        private Dictionary<string, Move> ResourceCache = new();
        //private Dictionary<string, PokeApiNet.Type> MoveTypeCache = new();
        private bool IsLoaded = false;

        public CacheService(PokeApiData apiLayer)
        {
            ApiLayer = apiLayer;
        }

        // Preload all moves into the dictionary and mark as loaded
        public async Task EnsureMovesLoadedAsync()
        {
            if (IsLoaded)
                return; // Skip if already loaded

            // Fetch all move resources
            var allMoves = await ApiLayer.GetAllMovesAsync();

            // Add to the dictionary
            ResourceCache = allMoves.ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase);

            // Mark as loaded
            IsLoaded = true;
        }

        // Get a move by its name
        public async Task<Move?> GetMoveByNameAsync(string moveName)
        {
            await EnsureMovesLoadedAsync();
            return ResourceCache.TryGetValue(moveName, out var move) ? move : null;
        }

        // Get multiple moves by their names
        public async Task<IEnumerable<Move>> GetMovesByNamesAsync(IEnumerable<string> names)
        {
            await EnsureMovesLoadedAsync();
            return names.Select(name => ResourceCache.TryGetValue(name, out var move) ? move : null)
                        .Where(m => m is not null)!;
        }

        public async Task<IEnumerable<Move>> GetMovesByPokemonAsync(Pokemon pokemon)
        {
            await EnsureMovesLoadedAsync();
            return pokemon.Moves.Select(x => ResourceCache.TryGetValue(x.Move.Name, out var move) ? move : null)
                        .Where(m => m is not null)!;
        }
    }

}
