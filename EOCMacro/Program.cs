using EOCMacro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
	public static void Main(string[] args)
	{

		var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
		var configuration = builder.Build();
		var serviceProvider = new ServiceCollection()
		.AddSingleton<IConfiguration>(_ => configuration)
		.AddSingleton<ISpellComboService, SpellComboService>()
		.AddSingleton<ISpellComboMacroService, SpellComboMacroService>()
		.BuildServiceProvider();

        var spellComboService = serviceProvider.GetService<ISpellComboService>();
		var macroService = serviceProvider.GetService<ISpellComboMacroService>();

		if (spellComboService != null && macroService != null)
		{
			try
			{
				var allSpellComboCombinations = spellComboService.FindSpellCombo("EAFW", 4);
				macroService.GenerateSpellCombosMacroFiles(allSpellComboCombinations);
				Console.Write("Macro files generated successfully");
			}
			catch(Exception e)
            {
				Console.Write($"Exception: {e.Message}");
            }
		}

        else {
			Console.Write("Service initialization failed");
        }
	}
}
