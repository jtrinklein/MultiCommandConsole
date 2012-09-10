#if NDESK_OPTIONS
namespace NDesk.Options
#else
namespace Mono.Options
#endif
{
	public delegate void OptionAction<TKey, TValue>(TKey key, TValue value);
}