namespace EOCMacro
{
    public class SpellComboService : ISpellComboService
    {
		private IList<string> spellComboCombination;

		public SpellComboService()
        {
			spellComboCombination = new List<string>();

		}
		public IList<string> FindSpellCombo(String str, int size)
		{
			char[] result = new char[size];
			AllSpellComboPermutation(str, result, 0, size);
			return spellComboCombination;
		}

		private void AllSpellComboPermutation(String str, char[] result, int n, int size)
		{
			for (int i = 0; i < size; ++i)
			{
				result[n] = str[i];
				if (n == size - 1)
				{
					var ans = new String(result);
					spellComboCombination.Add(ans);
				}
				else
				{
					AllSpellComboPermutation(str, result, n + 1, size);
				}
			}
		}
	}
}
