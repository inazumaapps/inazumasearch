VERSION = '0.7.2'
MSBUILD = 'C:\\Program Files (x86)\\MSBuild\\14.0\\Bin\\MSBuild.exe'

DEST_ZIP = "out/InazumaSearch-#{VERSION}.zip"
DEST_ZIP_SRC = "out/InazumaSearch-#{VERSION}-Source.zip"
DEST_ZIP_PORTABLE = "out/InazumaSearch-#{VERSION}-Portable.zip"

RELEASE_EXE = "InazumaSearch/bin/Release/x86/InazumaSearch.exe"
RELEASE_PORTABLE_EXE = "InazumaSearch/bin/Release_Portable/x86/program/InazumaSearch.exe"

SRCS = FileList['InazumaSearch/**/*']
SRCS.exclude('InazumaSearch/bin/**/*')
SRCS.exclude('InazumaSearch/obj/**/*')

task :default => :zip

desc "-"
task :zip => ['zip:standard', 'zip:source', 'zip:portable']

desc "-"
task 'zip:standard' => DEST_ZIP

desc "-"
task 'zip:source' => DEST_ZIP_SRC

desc "-"
task 'zip:portable' => DEST_ZIP_PORTABLE

file DEST_ZIP => [RELEASE_EXE] do |task|
    # zipファイルを作成
    make_zip("Release", DEST_ZIP)

    # zipファイルの内容を一度展開
    rm_r 'out/content' if File.exist?('out/content')
    sh %Q|7z x "#{DEST_ZIP}" -o"out/content" |

    # ExePress用のiniファイルを作成
    express_enc = 'UTF-16LE'
    inibody = File.read('InazumaSearch_exepress_template.ini', encoding: express_enc, mode: 'rb')
    inibody.gsub!('${CHDIR}'.encode(express_enc), Dir.pwd.gsub('/', '\\').encode(express_enc));
    inibody.gsub!('${VERSION}'.encode(express_enc), VERSION.encode(express_enc));
    File.write("out/InazumaSearch_exepress_#{VERSION}.ini", inibody, encoding: express_enc, mode: 'wb')
end

file DEST_ZIP_PORTABLE => [RELEASE_PORTABLE_EXE, __FILE__] do |task|
    # ポータブル版のzipファイルを作成
    make_portable_zip("Release_Portable", DEST_ZIP_PORTABLE)
end

file DEST_ZIP_SRC => [__FILE__] do |task|
    # ソースzipを作成
    rm task.name if File.exist?(task.name)
    sh %Q|7z a -i!InazumaSearch -i!InazumaSearch_Debug -i!portableLaunch -i!PluginSDK -i!Plugins -i!InazumaSearch.sln -i!restarter -x!InazumaSearch/bin -x!InazumaSearch_Debug/bin -x!PluginSDK/bin -xr!obj -xr!.vs -x!InazumaSearch/Icon -x!InazumaSearch/*.ico "#{task.name}"|
    sh %Q|7z a -i!InazumaSearch/Icon/inazumasearch-icon.ico "#{task.name}"|
end

desc "-"
task :build => [RELEASE_EXE, RELEASE_PORTABLE_EXE]

file RELEASE_EXE => [__FILE__] + SRCS.to_a do
    # リリース版ビルド
    sh %Q|"#{MSBUILD}" /maxcpucount /t:Rebuild "/p:Configuration=Release;Platform=x86"|
end

file RELEASE_PORTABLE_EXE => [__FILE__] + SRCS.to_a do
    # リリース版（Portable）ビルド
    sh %Q|"#{MSBUILD}" /maxcpucount /t:Rebuild "/p:Configuration=Release (Portable);Platform=x86"|
end

desc "-"
task :clean do
    # クリーン
    sh %Q|"#{MSBUILD}" /maxcpucount /t:Clean "/p:Configuration=Release;Platform=x86"|
    sh %Q|"#{MSBUILD}" /maxcpucount /t:Clean "/p:Configuration=Release (Portable);Platform=x86"|
    if Dir.exist?('InazumaSearch/bin/Release/x86') then
    	rm_r 'InazumaSearch/bin/Release/x86'
    end
    
    if Dir.exist?('InazumaSearch/bin/Release_Portable/x86') then
    	rm_r 'InazumaSearch/bin/Release_Portable/x86'
    end
end

def make_zip(platform, dest_zip_path)
	begin
	    rm dest_zip_path if File.exist?(dest_zip_path)
	    
	    cd "InazumaSearch/bin/#{platform}/x86" do
	        mkpath 'plugins'
	        sh %Q|7z a -x!data -x!NLog.config -x!GPUCache -x!*.vshost.exe -x!*.vshost.exe.* -x!_DebugExeTemporary -x!locales -x!swiftshader -x!debug.log "../../../../#{dest_zip_path}" .|
	        sh %Q|7z a "../../../../#{dest_zip_path}" locales/ja.pak|
	    end
	    cd 'InazumaSearch' do
	        sh %Q|7z a "../#{dest_zip_path}" html|
	    end
    rescue
    	rm dest_zip_path if File.exist?(dest_zip_path)
    	raise $!
    end
end

def make_portable_zip(platform, dest_zip_path)
	begin
	    rm dest_zip_path if File.exist?(dest_zip_path)
	    
	    cd "InazumaSearch/bin/#{platform}/x86" do
	    	# exe名リネーム
	    	rm 'InazumaSearchPortable.exe' if File.exist?('InazumaSearchPortable.exe')
	    	mv 'portableLaunch.exe', 'InazumaSearchPortable.exe'
	    
	    	# pluginsディレクトリ追加
	        mkpath 'program/plugins'
	        
	        # 圧縮
	        sh %Q|7z a -x!*.pdb -x!portableLaunch.exe.config -x!data -x!program/NLog.config -x!program/GPUCache -x!program/*.vshost.exe -x!program/*.vshost.exe.* -x!program/_DebugExeTemporary -x!program/locales/*.pak -x!program/swiftshader -x!program/debug.log "../../../../#{dest_zip_path}" .|
	        sh %Q|7z a "../../../../#{dest_zip_path}" program/locales/ja.pak|
	    end
	   	    
	    
	    # tmpディレクトリを作成し、その中にhtmlフォルダをコピー
	    mkpath 'out/_portable_tmp/program'
	    cp_r 'InazumaSearch/html', 'out/_portable_tmp/program'
	    
	    cd 'out/_portable_tmp' do
	        sh %Q|7z a "../../#{dest_zip_path}" program|
	    end

    rescue
    	rm dest_zip_path if File.exist?(dest_zip_path)
    	raise $!
    ensure
    	begin
    		rm_r 'out/_portable_tmp'
    	rescue
    	end
    end
end
