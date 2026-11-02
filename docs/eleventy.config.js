import { govukEleventyPlugin } from '@x-govuk/govuk-eleventy-plugin'

const repositoryUrl = 'https://github.com/DFE-Digital/manage-entitlement-funding-for-early-years-providers'
const pathPrefix = process.env.ELEVENTY_ENV === 'production'
  ? '/manage-entitlement-funding-for-early-years-providers/'
  : '/'

export default function (eleventyConfig) {
  eleventyConfig.addPassthroughCopy('assets/images')
  eleventyConfig.ignores.add('node_modules/**')
  eleventyConfig.ignores.add('eleventy.config.js')

  // Markdown source links also need to resolve to the published HTML pages.
  eleventyConfig.addTransform('markdown-links', function (content) {
    if (this.page.outputFileExtension !== 'html') return content
    return content.replace(/href="([^"]+\.md(?:#[^"]*)?)"/g, (match, href) => {
      if (/^[a-z][a-z\d+.-]*:|^\/\//i.test(href)) return match
      const sourcePath = this.page.inputPath.replaceAll('\\', '/').replace(/^\.\//, '')
      const url = new URL(href, `https://docs.invalid${pathPrefix}${sourcePath}`)
      const pathname = url.pathname.replace(/README\.md$/, '').replace(/\.md$/, '/')
      return `href="${pathname}${url.hash}"`
    })
  })

  eleventyConfig.addPlugin(govukEleventyPlugin, {
    titleSuffix: 'Manage entitlement funding for early-years providers',
    stylesheets: ['/assets/styles.css'],
    templates: {
      error404: {
        content: 'Check the web address or return to [the homepage](/) to find the information you need.'
      },
      sitemap: false
    },
    header: {
      logotype: {
        html: '<img src="/assets/images/department-for-education_white.png" alt="Department for Education">'
      }
    },
    serviceNavigation: {
      serviceName: 'Manage entitlement funding for early-years providers',
      navigation: [
        {
          text: 'Tutorials',
          href: '/tutorials/'
        },
        {
          text: 'Reference',
          href: '/reference/'
        }
      ]
    },
    footer: {
      logo: false,
      meta: {
        items: [
          {
            href: repositoryUrl,
            text: 'GitHub repository'
          }
        ]
      }
    }
  })

  return {
    pathPrefix,
    templateFormats: ['md', 'njk', 'scss', '11ty.js'],
    dir: {
      input: '.',
      output: '../artifacts/docs'
    }
  }
}
