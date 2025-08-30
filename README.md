# WindowerLauncher

## Introduction

[WindowerLauncher](https://github.com/Kaiconure/WindowerLauncher/) is a tool to launch Windower4 while managing multiple POL login profiles. It is mainly targeted at anyone with 5+ accounts, to simplify working around the 4-account limit set by POL.

It works by safely organizing multiple versions of your `login_w.bin` file, with backups, to allow you to easily switch between named profiles.

**WindowerLauncher is a command line tool**, but that's only a concern when setting up and managing profiles. Once you're configured, you won't need to worry about using the command line again unless you want to make changes.

> Check out our [Releases page](https://github.com/Kaiconure/WindowerLauncher/releases) to download the latest version and to see what's new.

## Setup and Configuration

Check out the WindowerLauncher wiki for information and instructions. Here are some starting points for you to browse:

**Intro**
- [Introduction to WindowerLauncher](https://github.com/Kaiconure/WindowerLauncher/wiki)

**Quick Start**
- [Walk-through: Six-account setup](https://github.com/Kaiconure/WindowerLauncher/wiki/Six-Account-Setup)
- [Command line cheat sheet](https://github.com/Kaiconure/WindowerLauncher/wiki/Command-line-cheat-sheet)
  
**General Setup**

*If you didn't follow the quick-start guide, or are curious about the individual steps:*
- Step 1: [Download and Installation](https://github.com/Kaiconure/WindowerLauncher/wiki/Installation-Guide)
- Step 2: [Saving your profile](https://github.com/Kaiconure/WindowerLauncher/wiki/Saving-your-profile)
- Step 3: [Creating and saving secondary profiles](https://github.com/Kaiconure/WindowerLauncher/wiki/Creating-new-profiles)


## FAQ

#### What if I have multiple PlayOnline locales installed?

All WindowerLauncher commands support a `-locale:<LOCALE>` command line option. Supported values for locale are `US`, `JP`, and `EU`. By default, WindowerLauncher will search for installations *in that order*, and use the first one it finds. If you want to override this, just add the specific locale you want to use.

For example, let's say you're using the JP client. You might run commands like the following:

```bash
WindowerLauncher.exe new -locale:JP
WindowerLauncher.exe save -name:Secondary -locale:JP
```

You batch files will preserve the locale they were created with.

Note that I recommend *not* creating profiles across locales with the same names. While your underlying POL login data files will be fine, the launcher batch files created with WindowerLauncher could get overwritten.

