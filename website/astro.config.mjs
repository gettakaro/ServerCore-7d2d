// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import starlightLinksValidator from 'starlight-links-validator';
import changelog from './scripts/changelog.mjs';

const repo = 'https://github.com/gettakaro/ServerCore-7d2d';

// https://astro.build/config
export default defineConfig({
	site: 'https://gettakaro.github.io',
	base: '/ServerCore-7d2d',
	integrations: [
		changelog(),
		starlight({
			title: 'ServerCore',
			description: 'Documentation for ServerCore, the open-source server mod for 7 Days to Die.',
			social: [{ icon: 'github', label: 'GitHub', href: repo }],
			editLink: { baseUrl: `${repo}/edit/main/website/` },
			routeMiddleware: './src/routeData.ts',
			plugins: [starlightLinksValidator()],
			sidebar: [
				{
					label: 'Start here',
					items: [
						{ label: 'Introduction', slug: 'index' },
						{ slug: 'start-here/installation' },
						{ slug: 'start-here/migrating-from-prismacore' },
						{ slug: 'start-here/faq' },
					],
				},
				{
					label: 'Features',
					items: [
						{ slug: 'features/advanced-claims' },
						{ slug: 'features/claimcreator' },
						{ slug: 'features/reset-regions' },
						{ slug: 'features/pvpve-configuration' },
						{ slug: 'features/location-based-buffs' },
						{ slug: 'features/vip-modguard' },
						{ slug: 'features/settings-file' },
					],
				},
				{
					label: 'Commands',
					items: [{ slug: 'commands/console-commands' }, { slug: 'commands/chat-commands' }],
				},
				{
					label: 'Project',
					items: [
						{ slug: 'project/changelog' },
						{ slug: 'project/compatibility' },
						{ label: 'Contributing', link: `${repo}/blob/main/CONTRIBUTING.md` },
					],
				},
			],
		}),
	],
});
