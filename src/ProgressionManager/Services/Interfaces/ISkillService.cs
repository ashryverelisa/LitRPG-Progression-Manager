using System.Collections.Generic;
using ProgressionManager.Models.Skills;
using ProgressionManager.Models.WorldRules;

namespace ProgressionManager.Services.Interfaces;

public interface ISkillService
{
    IEnumerable<SkillDefinition> GetDefaultSkills();
    SkillDefinition CreateActiveSkill(string name = "NEW_SKILL");
    SkillDefinition CreatePassiveSkill(string name = "NEW_PASSIVE");
    SkillDefinition CloneSkill(SkillDefinition skill);
    StatusEffect CreateStatusEffect(string name = "NEW_EFFECT");

    void ValidateSkillFormulas(
        SkillDefinition skill,
        IEnumerable<StatDefinition> stats,
        int previewLevel,
        int skillRank);

    SkillTree CreateSkillTree(string name = "NEW_TREE");
    SkillTreeNode CreateSkillTreeNode(SkillDefinition? skill = null, int tier = 0);
}
