import os
import sys
import subprocess
import yaml
import argparse
import platform

from tkinter.filedialog import askopenfilename

import argparse

parser = argparse.ArgumentParser(description='Start Unity Editor with ROS2 support')
parser.add_argument('--select-editor', action='store_true', help='select executable for the editor')
args = parser.parse_args()

config_filename = 'ros_config.yaml'

# Defaults
if platform.system() == 'Windows':
    yaml_data = {
        'unity_editor_filename': None,
        'c_libs_asset_path': 'Plugins\\Windows\\x86_64'
    }
else:
    yaml_data = {
        'unity_editor_filename': None,
        'c_libs_asset_path': 'Plugins/Linux/x86_64'
    }

if os.path.isfile(config_filename):
    with open(config_filename) as stream:
        try:
            yaml_data = yaml.safe_load(stream)
        except yaml.YAMLError as exc:
            print(exc)
else:
    print('Config file does not exsist, creating!')
    with open(config_filename, 'w') as outfile:
        yaml.dump(yaml_data, outfile, default_flow_style=False)

if yaml_data['unity_editor_filename'] == None or args.select_editor:
    if platform.system() == 'Windows':
        default_editor_folder = "/Program Files/Unity/Hub/Editor"
        editor_filetype = "Unity.exe"
    else:
        default_editor_folder = "~/Unity/Hub/Editor"
        editor_filetype = "Unity"

    filename =  askopenfilename(initialdir = default_editor_folder,
                                title = "choose unity editor",
                                filetypes = (("Editor", editor_filetype),("all files","*.*")))

    yaml_data['unity_editor_filename'] = filename
    if filename == '':
        raise Exception('No Editor Selected!')

    with open(config_filename, 'w') as outfile:
        yaml.dump(yaml_data, outfile, default_flow_style=False)

unity_editor_filename = yaml_data['unity_editor_filename']
asset_directory = os.path.dirname(os.path.realpath(sys.argv[0]))

project_directory = os.path.abspath(os.path.join(asset_directory, os.pardir))

if platform.system() == 'Windows':
    library_path = project_directory + '\\Assets\\' + yaml_data['c_libs_asset_path']
    if not (library_path in os.environ['PATH']):
        os.environ['PATH'] = os.environ['PATH'] + ';' + library_path + ';'
else:
    library_path = project_directory + '/Assets/' + yaml_data['c_libs_asset_path']
    if 'LD_LIBRARY_PATH' in os.environ:
        os.environ['LD_LIBRARY_PATH'] = os.environ['LD_LIBRARY_PATH'] + ':' + library_path
    else:
        os.environ['LD_LIBRARY_PATH'] = library_path

    if not (library_path in os.environ['PATH']):
        os.environ['PATH'] = os.environ['PATH'] + ':' + library_path


subprocess.call([unity_editor_filename, '-projectPath', project_directory])
