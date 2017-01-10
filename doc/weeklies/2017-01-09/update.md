# Current State

It works! Kind of. If you discount the fact that the AI doesn't learn, the gameplay is drop-dead simple, and you have to mash spacebar six times every turn, it's technically playable and winnable. So that's good!

Here are where the various aspects are at:

* AI Learning: Nonexistent; there is one and only one AI, and it's hard-coded.
* Arena Gameplay: Technically playable. The action system is a little opaque, and the way that passing works requires you to hit spacebar once for every weapon you have, every turn, if you don't want to move. Could be improved on a lot!
* Mech Customization: Mechs are preconstructed, and there's only a very limited subset of the final intended parts available. You cannot customize your own mech.
* Tournament System: Mostly working! However, there's no provision for character death yet. Also, higher rounds are still round-robin, whereas they should be a double-elimination bracket.
* Replay System: AI v. AI replays work, but Player v. AI replays do not.
* UI: It is terrifying.

# Gifs

## AI v. AI Match

Demonstrates the generation of an AI v. AI match, and the replay of the match from the competitor screen. Matches between AIs are stored and replayable, so that you can see what kind of AI/mechs they use and tailor your tactics appropriately.

I mean, right now the mechs are fixed, and the AI is hard-coded, but it'll be useful later!

![The UI could use some work...](https://github.com/MoyTW/WinterIsCoding/blob/master/doc/weeklies/2017-01-09/gen_match_ai_v_ai.gif)

## Plyaer v. AI Match

A full player v. AI match. Unfortunately there's no message log so it's really hard to tell what's goin on!

![This is like a minute long](https://github.com/MoyTW/WinterIsCoding/blob/master/doc/weeklies/2017-01-09/full_player_match.gif)

# This Week

* Add a variety of weapons, as per the outline
* Expose weapon-building functions/objects as an enumerable structure, such that they can be iterated over
* AI mech building, utilizing genetic algorithm
  - Structure construction as a series of genes
  - Each gene is a location + item pair
  - Fixed-length chromosome (some genes will not be expressed)
  - Fitness function is playing match against a baseline enemy
  - First-pass; only needs to function as proof-of-concept