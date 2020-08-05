# NablaUtils

```bash
git submodule add https://github.com/samo4/NablaUtils.git
```

how to clone a project that includes this submodule:

```
git clone --recurse-submodules https://github.com/chaconinc/MainProject
```

pull current submodule

```
git submodule update --init --recursive.
```

update all submodules?

```
git submodule foreach git pull origin master
```
