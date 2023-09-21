# Contributing
Thank you so much for considering contributing to the project!

Before continuing, make sure that you read the 
[Rust's code of conduct](https://www.rust-lang.org/policies/code-of-conduct).

> Are you an artist? Read the [For artists](#for-artists) section.

## Table of contents

1. [Getting Started](#getting-started)
2. [Working with velocities and units](#working-with-velocities-and-units)
3. [Reporting bugs](#reporting-bugs)
    - [For Windows users](#for-windows-users)
4. [Providing feedback](#providing-feedback)
5. [For artists](#for-artists)
    - [For pixel artists](#for-pixel-artists)
    - [For music artists](#for-music-artists)


## Getting Started
If you want to contribute to the project, you'll need to have

- [Godot .NET 4.1.1](https://godotengine.org/download) installed.
- A compatible C# IDE (we recommend
    [Visual Studio Code](https://code.visualstudio.com/Download)).
- And that's it!

Also, make sure to follow the [coding style](CODING_STYLE.md) of the project.


> **Note**
> Please consider using C# over GDScript. GDScript is a great
> language, but it's not as complete as C# is.

### For Windows users
If you're using Windows, please make sure to use the same line endings that the
project uses. You can do this by configuring git to use `lf` line endings by
default by running the following command in the terminal:

```bash
git config --global core.autocrlf input
```

## Working with velocities and units
The game unit's base is 16 x 16 pixels. This means that, if you want to edit
some velocity, you'll have to think in terms of pixels per second.

You can use a simple formula to convert from units to pixels per second:

```
pixels_per_second = units_per_second * 16
```


## Reporting bugs
A **bug** is something that clearly *and objectively* does not work as intended.
Examples of bugs are:

- The game crashes to desktop when I load a level.
- The camera limits are not correct.
- A piece of text is overlappint another piece of text.
- The game slows down when I do X.

To track bug reports, we use GitHub **issues**. If you find a bug and want to
report it, please keep in mind the following:

- **Search for existing issues.** Before opening a new issue, please search
    if the bug has already been reported. If it has, you can add a comment
    to the existing issue instead of opening a new one.

- **Use a clear and descriptive title.** A good title is short, descriptive,

- **Include steps to reproduce the bug.** If you can, include a video or
    screenshots of the bug.

- **We may ask you for more information.** If we need more information to
    reproduce the bug, we'll ask you for it. Please, be patient and provide
    us with the information we need.

If we cannot reproduce the bug, we may close the issue or tag it as
low priority.

If the bug you want to report is too general (for example, *the game is too
slow*), please open a discussion instead of an issue or talk to us on
[Discord](https://discord.gg/HyycTY5Nrj).

## Providing feedback
If you wish to:
- Provide *subjective* feedback on the game (about the UI, the sprites, the
    music, etc.).
- Suggest a new feature to be added to the game.
- Suggest a change to an existing feature.

then please open a discussion instead of an issue or talk to us on
[Discord](https://discord.gg/HyycTY5Nrj). We'll be glad to hear your feedback!

When openning a discussion, please keep in mind the following:

- Use the search function to see if your idea has already been suggested. Or
    if there's already a thread about the topic you want to talk about and want
    to raise the discussion again.
- If you want to suggest a new feature, please be as descriptive as possible.
    Tell us what the feature is, how it works, how it would be implemented,
    etc.
- If you want to suggest a change to an existing feature, please tell us why
    you want to change it and then tell us how you would change it.


## For artists
Welcome to the project! We're glad that you're here!

I'm sure that, with your talent, we'll be able to make the game look even
better!



<details>
<summary>For pixel artists</summary>

### Working with spritesheets
If you want to edit some sprite, please
**DO NOT EDIT THE SPRITESHEETS DIRECTLY**. Instead, edit the individual sprites
in the `.scraps` directory, and then pack the spritesheets again using
TexturePacker.

If you haven't bought TexturePacker, you can edit the sprites and open a pull
request, and we'll pack the spritesheets for you (if you want to).

If you want to use the free version TexturePacker, please don't. Everything
is already configurated to work with Godot, so you don't need to do anything
else.


### Creating a new avatar
If you want to add a new avatar to the game, please open an issue first
or talk to us on [Discord](https://discord.gg/HyycTY5Nrj).

If you have permission to add a new avatar, follow these steps:

1. Create a new directory in `.scraps/avatars/` with the next number in the
    sequence. For example, if the last avatar is `3`, the new avatar
    should be `4` (`.scraps/avatars/avatar-4`).

2. Copy all of the contents of the `.scraps/avatars/avatar-0` directory to
    the new directory.

3. Edit the sprites in the new directory.

4. Open a pull request with the changes. Or, you can send us the sprites
    directly on [Discord](https://discord.gg/HyycTY5Nrj) as a `.zip` file.

5. We'll pack the spritesheets for you, and add the avatar to the game.

> **Note**
> Please give the avatar a name. We are not as creative as you are!

#### Avatar sprites guidelines

- The avatar sprites canvas size have to be `24 x 24`.
- Use just `22 x 22` pixels for the avatar. The outter pixels are used for
    the avatar's outline.
- The avatar's base size in the game (and hitbox) is `8 x 16` pixels.
- For jumping and falling animations, keep the head of the avatar in the
    same position.
- For running animations, move the head of the avatar up and down every two
    frames.
- We suggest you to use just two frames for the jumping animation and three
    frames for the falling animation.

#### Suggested software
We suggest you to use [Krita](https://krita.org/en/download/krita-desktop/)
or [Aseprite](https://www.aseprite.org/#download) to create the sprites.

Please, do not use Asesprite if you're using a pirated version of it.

</details>


<details>
<summary>For music artists</summary>

### Working with the music
Please do not add new music to the project without asking first.

If you want to add a new music, please open an issue first, and we'll discuss
if it's a good idea to add it to the project. Or you can talk to us on
[Discord](https://discord.gg/HyycTY5Nrj).

### Creating a new music

If you have permission to add a new music, please follow the next steps:

1. Create the music in any software you like (unless you're using pirated
    software, in that case, please don't).

2. Export the music as a `.ogg` file.

3. Send us the music on [Discord](https://discord.gg/HyycTY5Nrj) or open a
    pull request with the changes.

4. We'll add the music to the game.

> **Note**
> Mention us the category of the music (by example gameplay music, menu music,
> etc.)

#### Music guidelines

- The music has to be in `.ogg` format.
- The music has to loop seamlessly.
- Do not exceed 5 or 6 minutes of duration.
- All the music has to be composed by you, do not use other people's samples or
    melodies.
- The license of the music has to be compatible with the project's license
    (you can use Creative Commons licenses by example).
- The music has to be 8-bit or 16-bit style. No modern music, this is not
    Just Shapes & Beats.

#### Suggested software
We suggest you to use [LMMS](https://lmms.io/download/) or
[FamiTracker](http://famitracker.com/downloads.php) to create the music.

If you want to edit the music, you can use
[Audacity](https://www.audacityteam.org/download/).

</details>
