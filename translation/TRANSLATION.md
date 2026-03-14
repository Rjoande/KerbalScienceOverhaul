# Translating Kerbal Science Overhaul

Kerbal Science Overhaul contains more than 20,000 science result strings. Manual translation without tooling support would be extremely time-consuming.

To make localization feasible, the project includes a workflow designed to work with modern language models. Using these tools, contributors can translate large batches of science results while preserving terminology, structure, and tone.

Translations are generated from the English source strings.

---

## Contribution Options

There are two ways to contribute to localization.

A **dictionary template** is available in the repository to help contributors prepare terminology for new languages.

### Option 1 — Provide a Translation Dictionary

The easiest way to help is by preparing a translation dictionary for your language.

The dictionary should be a JSON file containing:

- translations of celestial body names
- biome names
- technical Kerbal-universe terminology
- scientific vocabulary used in experiment results

These dictionaries ensure consistent terminology across thousands of strings.

Once a dictionary is available, the project author can generate the full localization dataset using the AI workflow.

This is often the most efficient way for native speakers to contribute.

---

### Option 2 — Run the Full Translation Workflow

Advanced contributors may run the translation workflow themselves.

The system was designed to work with:

- GPT-5.2 or later
- **Thinking / reasoning mode**
- a **closed project environment**

The repository includes a [**ready-to-use project instruction file**](https://github.com/Rjoande/KerbalScienceOverhaul/blob/main/translation/LLM_translation_instructions.txt)
(in English, therefore usable for any language).

The project environment should include:

- the translation instructions
- the science result dataset
- the destination language dictionary

The dictionary provides canonical translations for body names, biomes, and technical terms used throughout the dataset.

---

## Translation Workflow

If the project instructions are used correctly, **no additional prompt is normally required**.

The typical workflow is:

1. Start a new chat inside the translation project.
2. Paste a block of localization strings.
3. The AI returns the translated keys.

Recommended batch size:

- keep blocks **below ~8–10k characters**

This improves reliability and formatting consistency.

---

### Chat Management

Large chats gradually become less reliable at respecting instructions.

For best results:

- start a **new chat every ~1000 translated lines**

This keeps the model focused and avoids drift in formatting rules.

---

### Reinforcement Prompt (Optional)

In some cases — especially at the start of a chat — it may help to include a short reinforcement instruction before pasting the text block.

Example:

```
Always use the project dictionary to translate place names.
```

This reminder helps ensure that body names and biome names remain consistent.

---

## Dictionary Format

The translation dictionary is a JSON file.

It typically contains translations for:

- celestial bodies
- biome names
- scientific terminology
- recurring Kerbal expressions

Only the destination language should be included in the dictionary.
The source language is always English.

---

## Contributing a Translation

If you would like to contribute a translation:

1. Prepare a language dictionary **or**
2. Run the translation workflow using the provided instructions

Then open a pull request or contact the project author.

Even partial translations or terminology dictionaries are extremely valuable.
