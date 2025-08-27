# WindowerLauncher
## 1. Introduction

[WindowerLauncher](https://github.com/Kaiconure/WindowerLauncher/) is a tool to launch Windower4 while managing multiple POL login profiles. It's mainly targeted at anyone with 5+ accounts, to simplify working around the 4-account limit set by POL.

It works by safely organizing multiple versions of your `login_w.bin` file, with backups, to allow you to easily switch between named profiles.

**WindowerLauncher is a command line tool**, but that's only a concern when setting up and managing profiles. Once you're configured, you won't need to worry about using the command line again unless you want to make changes.

## 2. Installation

Get the [latest release](https://github.com/Kaiconure/WindowerLauncher/releases/), and extract it directly into your Windower4 installation folder. You should have WindowerLauncher.exe sitting side-by-side with Windower.exe.

If you don't know where Windower was installed, you can check the following locations:

- *C:\Program Files (x86)\Windower4*
- *C:\Windower4*
- *C:\Users\\&lt;**your-account>**\Downloads\* - This is for anyone who downloaded the executable and just ran it straight out of your Downloads. :sweat_smile:

If none of these work for you, right-click your Windower shortcut, right-click "Windower Launcher" (unrelated to WindowerLauncher, sorry for the confusion), left-click on "Properties", and check the "Target" box.

## 3. How it works

First of all, WindowerLauncher *always* backs up your `login_w.bin` before replacing it. The last 10 versions can be found in timestamped files in a subdirectory named `.wlbackups` under where your `login_w.bin` file is located. By default, that would be something like this for a Steam install:

- *C:\Program Files (x86)\Steam\steamapps\common\FFXINA\SquareEnix\PlayOnlineViewer\usr\all\\.wlbackups*

You can manually restore from these backups if necessary. You may also consider creating your own original backup somewhere safe, just in case you decide to go all the way back at some point.

Now that that's out of the way, let's run through profile setup via example as that's probably the easiest way to get started.

### Creating your first profile

Ok, let's create a profile. We'll call our it`Main`, but you can use any name you'd like -- as long as it uses valid file name characters (no colons or slashes, for example). For simplicity, I also recommend avoiding spaces unless you know how to handle them in the command line.

Anyway, the pre-first step (Step #0?) is to be sure POL is set up the way you want for this profile. WindowerLauncher is all about switching easily between profiles, but it's still your job to get those profiles set up to begin with. I'm going to assume that's already the case, since you're likely signing in somehow today.

Now, let's go ahead and create our profile.

1. Browse to the Windower4 install folder in Windows Explorer. This is the same folder you identified in the **Installation** section above.
2. Open a Windows command prompt in your Windower4 install folder. The easiest way to do this is to double-click the `WindowerLauncher Command Prompt.cmd` file that installed with WindowerLauncher.
3. Now, let's create our new `Main` profile.
   - Type the following into the command prompt, and hit enter: `WindowerLauncher.exe save -name:Main`
4. You will now notice two things:
   - There will be a new batch file, `_run_profile.Main.bat`, in your Windower4 installation folder.
   - There will be a new bin file alongside your `login_w.bin` named `login_w_Main.bin`.
   - A desktop shortcut to your new profile, `Main - WindowerLauncher.lnk`. This is what you will typically use to launch your new profile.
5. Anytime you want to run with this profile, just double-click either the desktop shortcut (preferred) or the `_run_profile.Main.bat` file. This will backup the currently active profile, activate the "Main" profile you just saved in POL, and launch Windower.
6. If you ever want to modify the profile, just make your changes in POL. Once done, follow steps 1-3 here **but change the command in #3** to this instead:
   - `WindowerLauncher.exe save -name:Main -force`
   - WindowerLauncher will not allow you to save over an existing profile unless you use the `-force` option.
7. You're good to go!

### Creating secondary profiles

Let's be honest, WindowerLauncher doesn't really get you anything if you're only going to have that first profile. Let's run through how to create secondary profiles to make it all worth it.

To start, you'll need a command prompt running in your Windower4 installation folder. Follow steps 1-2 from the "Creating your first profile" section above, then come back here.

Now that you're ready, let's set up your new profile:

1. Type the following into your command prompt:
   - `WindowerLauncher.exe new`
2. You will now have a new, empty `login_w.dat` file in your POL folder. Launch POL directly, via Windower.exe or via a native FFXI shortcut. **Do not use a WindowerLauncher profile batch file, as it would activate that profile which is not what you want here.**
3. Add the accounts you want for this second profile, and exit POL.
4. Now let's create our secondary WindowerLauncher profile, which for the sake of example we will call `Secondary`:
   - `WindowerLauncher.exe save -name:Secondary`
5. As before, you will see the following changes at this point. These should all look somewhat familiar:
   - There will be a new batch file, `_run_profile.Secondary.bat`, in your Windower4 installation folder.
   - There will be a new bin file alongside your `login_w.bin` named `login_w_Secondary.bin`.
   - A desktop shortcut to your new profile, `Secondary - WindowerLauncher.lnk`. This is what you will typically use to launch your new profile.
6. Anytime you want to run with this profile, just double-click either the desktop shortcut (preferred) or the `_run_profile.Secondary.bat` file. This will backup the currently active profile, activate the "Secondary" profile you just saved in POL, and launch Windower.
7. You're good to go!

You can go ahead and setup as many profiles as you want, following the procedure for creating secondary profiles.

## 4. FAQ

#### What if I have multiple PlayOnline locales installed?

All WindowerLauncher commands support a `-locale:<LOCALE>` command line option. Supported values for locale are `US`, `JP`, and `EU`. By default, WindowerLauncher will search for installations *in that order*, and use the first one it finds. If you want to override this, just add the specific locale you want to use.

For example, let's say you're using the JP client. You might run commands like the following:

```bash
WindowerLauncher.exe new -locale:JP
WindowerLauncher.exe save -name:Secondary -locale:JP
```

You batch files will preserve the locale they were created with.

Note that I recommend *not* creating profiles across locales with the same names. While your underlying POL login data files will be fine, the launcher batch files created with WindowerLauncher could get overwritten.

