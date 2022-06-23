using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EOCMacro
{
    public class SpellComboMacroService : ISpellComboMacroService
    {
		private readonly IConfiguration _config;
		public SpellComboMacroService(IConfiguration config) 
        {
			_config = config;
        }


		public void GenerateSpellCombosMacroFiles(IList<string> allSpellComboCombination)
		{
			//Split macro files based on 4 starting elements 
			var startparams = new string[] { "E", "A", "F", "W" };
			foreach (var param in startparams)
			{
				GenerateMacroFile(GetSpellComboStartingWith(param, allSpellComboCombination), param);
			}
		}

		public IList<string> GetSpellComboStartingWith(string c, IList<string> allSpellComboCombination)
		{
			return allSpellComboCombination.Where(a => a.Substring(0, c.Length).Equals(c)).ToList();
		}

		public void GenerateMacroFile(IList<string> combos, string name)
		{

			var macromodel = new MacroModel();

			bool isFast = true;
			var fileDirectory = @$"D:\Spell_Combo_Starting_{name.ToUpper()}";
			var fileName = isFast ? @$"{fileDirectory}_Fast.json" : @$"{fileDirectory}.json";

			var events = GetMacroEvents(combos, isFast);
			macromodel.Events = (List<MacroEvent>)events;

			string json = JsonConvert.SerializeObject(macromodel, Newtonsoft.Json.Formatting.Indented,
						  new JsonSerializerSettings
						  {
							  NullValueHandling = NullValueHandling.Ignore,
						  });
			File.WriteAllText(@$"D:\Spell_Combo_Starting_{name.ToUpper()}_Fast.json", json);
		}

		public IList<MacroEvent> GetMacroEvents(IList<string> combos, bool isFast = false)
		{
			// init
			var events = new List<MacroEvent>();
			var timestamp = 1000;

			foreach (var combo in combos)
			{
				//training begin
				StartTraining(events, ref timestamp, isFast);
				//start casting spell
				if (isFast)
				{
					CastSpellFast(events, ref timestamp, combo.ToString());
				}
				else
				{
					CastSpell(events, ref timestamp, combo.ToString());
				}
				//abort
				AbortTraining(events, ref timestamp);
			}

			return events;
		}

		public void StartTraining(List<MacroEvent> events, ref int timestamp, bool isFast = false)
		{
			// training challenge
			PerformClick(events, ref timestamp, 79.23999786376953, 82.0999984741211, 'C');
			timestamp += 2000;

			//training start
			PerformClick(events, ref timestamp, 93.44999694824219, 73.04000091552734, 'B');
			timestamp += 5000;

			// cancel auto (can be replaced by key stroke)
			PerformClick(events, ref timestamp, 16.149999618530273, 91.75, 'D');
			//give a couple of time to ensure all cooldown finished
			if (isFast)
			{
				timestamp += 4000;
			}
		}

		public void AbortTraining(List<MacroEvent> events, ref int timestamp)
		{
			timestamp += 1500;
			//training pause
			//to do: (can be replaced by key stroke)
			PerformClick(events, ref timestamp, 2.940000057220459, 91.37999725341797, 'P');
			timestamp += 1000;
			//training quit
			PerformClick(events, ref timestamp, 57.58000183105469, 67.30000305175781, 'X');
			timestamp += 3000;
		}

		public void PerformClick(List<MacroEvent> events, ref int timestamp, double x, double y, char? key = null)
		{
			if (key != null)
			{
				events.Add(new MacroEvent("KeyDown", timestamp, key.ToString()));
				timestamp += 150;
				events.Add(new MacroEvent("KeyUp", timestamp, key.ToString()));
			}
			else
			{
				events.Add(new MacroEvent("MouseDown", timestamp, null, 0, x, y));
				timestamp += 100;
				events.Add(new MacroEvent("MouseUp", timestamp, null, 0, x, y));
			}
		}


		public void PerformCast(List<MacroEvent> events, ref int timestamp, string spell)
		{
			events.Add(new MacroEvent("KeyDown", timestamp, spell));
			timestamp += 150;
			events.Add(new MacroEvent("KeyUp", timestamp, spell));
			timestamp += 200;
			events.Add(new MacroEvent("KeyDown", timestamp, spell));
			timestamp += 150;
			events.Add(new MacroEvent("KeyUp", timestamp, spell));
		}

		public void CastSpell(List<MacroEvent> events, ref int timestamp, string combo)
		{
			//todo: add logic to allow direct cast if spell is not at cooldown 
			int i = 0;
			var perunit_cooldown = 400;
			var airspell = 12 * perunit_cooldown;
			var earthspell = 15 * perunit_cooldown;
			var firespell = 18 * perunit_cooldown;
			var waterspell = 24 * perunit_cooldown;
			foreach (var x in combo)
			{
				switch (x)
				{
					case 'E': timestamp += earthspell; break;
					case 'A': timestamp += airspell; break;
					case 'F': timestamp += firespell; break;
					case 'W': timestamp += waterspell; break;
				}
				if (i == 0) { timestamp -= 4000; }

				PerformCast(events, ref timestamp, x.ToString());
				i++;
			}
		}

		public void CastSpellFast(List<MacroEvent> events, ref int timestamp, string combo)
		{
			int i = 0;
			var perunit_cooldown = 400;
			var spellElements = InitializeSpellElementModel(perunit_cooldown);
			foreach (var x in combo)
			{
				timestamp += spellElements[x].currentCoolDown;
				spellElements = DeductOtherSpellCoolDown(perunit_cooldown, spellElements[x].currentCoolDown, spellElements);
				spellElements[x].currentCoolDown = spellElements[x].initialCoolDown;
				//if (i == 0) { timestamp -= 4000; }

				PerformCast(events, ref timestamp, x.ToString());
				//Catharine specific - blizard will cause animation
				if (x == 'W')
				{
					timestamp += perunit_cooldown;
				}
				i++;
			}
		}


		public IDictionary<char, SpellElementModel> DeductOtherSpellCoolDown(int minimum_cooldown, int cooldown_deducted, IDictionary<char, SpellElementModel> spellElementModels)
		{
			foreach (var x in spellElementModels)
			{
				var currentCoolDown = spellElementModels[x.Key].currentCoolDown - cooldown_deducted;
				spellElementModels[x.Key].currentCoolDown = currentCoolDown <= minimum_cooldown ? minimum_cooldown : currentCoolDown;
			}
			return spellElementModels;
		}

		public IDictionary<char, SpellElementModel> InitializeSpellElementModel(int perunit_cooldown)
		{
			int airspell = 12 * perunit_cooldown;
			int earthspell = 15 * perunit_cooldown;
			int firespell = 18 * perunit_cooldown;
			int waterspell = 24 * perunit_cooldown;
			var spellElementModels = new Dictionary<char, SpellElementModel>()
		{
			{'E', new SpellElementModel { currentCoolDown = perunit_cooldown, initialCoolDown = earthspell } },
			{'A', new SpellElementModel { currentCoolDown = perunit_cooldown, initialCoolDown = airspell } },
			{'F', new SpellElementModel { currentCoolDown = perunit_cooldown, initialCoolDown = firespell } },
			{'W', new SpellElementModel { currentCoolDown = perunit_cooldown, initialCoolDown = waterspell } },
		};
			return spellElementModels;
		}
	}
}
