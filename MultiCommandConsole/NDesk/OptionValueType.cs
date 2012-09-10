#if NDESK_OPTIONS
namespace NDesk.Options
#else
namespace Mono.Options
#endif
{
	public enum OptionValueType
	{
		None,
		Optional,
		Required,
	}
}