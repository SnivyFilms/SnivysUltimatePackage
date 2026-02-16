import os
import re


def get_version_input(prompt):
    value = input(prompt).strip()
    if not re.match(r'^\d+\.\d+\.\d+$', value):
        print("Invalid format! Use X.Y.Z (e.g. 3.6.0)")
        return get_version_input(prompt)
    return value

def get_exiled_version_from_csproj(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    match = re.search(
        r'<PackageReference\s+Include="ExMod\.Exiled"\s+Version="([^"]+)"',
        content
    )

    if match:
        return match.group(1)

    return None

def ensure_or_update_tag(content, tag, value):
    pattern = rf'<{tag}>.*?</{tag}>'

    if re.search(pattern, content):
        return re.sub(pattern, f'<{tag}>{value}</{tag}>', content)
    else:
        # Insert into first PropertyGroup
        return re.sub(
            r'(<PropertyGroup[^>]*>)',
            rf'\1\n    <{tag}>{value}</{tag}>',
            content,
            count=1
        )


def update_csproj(filepath, plugin_version):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    content = ensure_or_update_tag(content, "Version", plugin_version)
    content = ensure_or_update_tag(content, "AssemblyVersion", f"{plugin_version}.0")
    content = ensure_or_update_tag(content, "FileVersion", f"{plugin_version}.0")
    content = ensure_or_update_tag(content, "InformationalVersion", plugin_version)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"Updated csproj: {filepath}")

def update_plugin_cs(filepath, plugin_version, exiled_version):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    changed = False
    new_version_block = f'''public override Version Version {{ get; }} =
            Version.Parse(Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "{plugin_version}");'''

    content, count = re.subn(
        r'public\s+override\s+Version\s+Version\s*\{[^}]*\}\s*=\s*[^;]+;',
        new_version_block,
        content,
        flags=re.MULTILINE
    )

    if count > 0:
        changed = True
    else:
        print(f"Version property not found in {filepath}")

    if exiled_version:
        exiled_parts = ", ".join(exiled_version.split("."))
        required_line = f'public override Version RequiredExiledVersion {{ get; }} = new Version({exiled_parts});'

        pattern = r'public\s+override\s+Version\s+RequiredExiledVersion\s*\{[^}]*\}\s*=\s*new Version\([^)]+\);'

        content, count = re.subn(pattern, required_line, content)

        if count == 0:
            insert_match = re.search(
                r'public\s+override\s+Version\s+Version\s*\{[^}]*\}\s*=\s*[^;]+;',
                content
            )
            if insert_match:
                insert_index = insert_match.end()
                content = content[:insert_index] + "\n\n        " + required_line + content[insert_index:]
            else:
                print(f"Could not insert RequiredExiledVersion in {filepath}")

        changed = True

    # Ensure using System.Reflection;
    if "using System.Reflection;" not in content:
        content = re.sub(
            r'(using\s+[^\n]+;\s*\n)',
            r'\1using System.Reflection;\n',
            content,
            count=1
        )
        changed = True

    if changed:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"Updated Plugin.cs: {filepath}")

def main():
    print("Enter new plugin version (X.Y.Z):")
    plugin_version = get_version_input("Plugin version: ")

    parent_dir = os.getcwd()

    for root, dirs, files in os.walk(parent_dir):
        for file in files:
            full_path = os.path.join(root, file)

            if file.endswith(".csproj"):
                print(f"Processing csproj: {full_path}")
                update_csproj(full_path, plugin_version)

                exiled_version = get_exiled_version_from_csproj(full_path)
                if exiled_version:
                    print(f"Detected Exiled version: {exiled_version}")
                else:
                    print("WARNING: Could not detect Exiled version.")

            elif file == "Plugin.cs":
                print(f"Processing Plugin.cs: {full_path}")

                # Try to find nearest csproj for Exiled version
                exiled_version = None
                for parent in os.listdir(root):
                    if parent.endswith(".csproj"):
                        exiled_version = get_exiled_version_from_csproj(os.path.join(root, parent))
                        break

                update_plugin_cs(full_path, plugin_version, exiled_version)


if __name__ == "__main__":
    main()
